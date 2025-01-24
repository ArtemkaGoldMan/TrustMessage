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
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MessageDTO> CreateMessageAsync(int userId, CreateMessageDTO messageDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var signature = MessageSigner.SignMessage(messageDto.Content, user.PasswordHash);

            var message = new Message
            {
                UserId = userId,
                Content = messageDto.Content,
                Signature = signature,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return new MessageDTO
            {
                Content = message.Content,
                Username = user.Username,
                CreatedAt = message.CreatedAt,
                IsVerified = MessageSigner.VerifyMessage(message.Content, message.Signature, user.PasswordHash)
            };
        }

        public async Task<IEnumerable<MessageDTO>> GetGeneralMessagesAsync()
        {
            return await _context.Messages
                .Include(m => m.User)
                .Select(m => new MessageDTO
                {
                    Content = m.Content,
                    Username = m.User.Username,
                    CreatedAt = m.CreatedAt,
                    IsVerified = MessageSigner.VerifyMessage(m.Content, m.Signature, m.User.PasswordHash)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageDTO>> GetPersonalMessagesAsync(int userId)
        {
            return await _context.Messages
                .Where(m => m.UserId == userId)
                .Select(m => new MessageDTO
                {
                    Content = m.Content,
                    Username = m.User.Username,
                    CreatedAt = m.CreatedAt,
                    IsVerified = MessageSigner.VerifyMessage(m.Content, m.Signature, m.User.PasswordHash)
                })
                .ToListAsync();
        }
    }
}

