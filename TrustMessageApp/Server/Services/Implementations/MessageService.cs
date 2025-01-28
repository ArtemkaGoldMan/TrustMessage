using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Security;
using Server.Services.Interfaces;

namespace Server.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly KeyManager _keyManager;

        public MessageService(ApplicationDbContext context, KeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        public async Task<IEnumerable<MessageDTO>> GetAllMessagesAsync()
        {
            return await _context.Messages
                .Include(m => m.User)
                .Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Username = m.User.Username,
                    Content = m.Content,
                    Signature = m.Signature,
                    CreatedAt = m.CreatedAt,
                    IsVerified = _keyManager.VerifyData(m.Content, m.Signature, m.User.PublicKey)
                }).ToListAsync();
        }

        public async Task<IEnumerable<MessageDTO>> GetUserMessagesAsync(string username)
        {
            return await _context.Messages
                .Where(m => m.User.Username == username)
                .Include(m => m.User)
                .Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Username = m.User.Username,
                    Content = m.Content,
                    Signature = m.Signature,
                    CreatedAt = m.CreatedAt,
                    IsVerified = _keyManager.VerifyData(m.Content, m.Signature, m.User.PublicKey)
                }).ToListAsync();
        }

        public async Task<MessageDTO> CreateMessageAsync(string username, CreateMessageDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new Exception("User not found");

            if (!PBKDF2Hasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid password");
            }

            var sanitizedContent = MessageSanitizer.Sanitize(request.Content);

            var decryptedPrivateKey = _keyManager.DecryptPrivateKey(user.PrivateKey, request.Password);
            var signature = _keyManager.SignData(sanitizedContent, decryptedPrivateKey);

            var message = new Message
            {
                UserId = user.Id,
                Content = sanitizedContent,
                Signature = signature,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return new MessageDTO
            {
                Id = message.Id,
                Username = user.Username,
                Content = message.Content,
                Signature = message.Signature,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
