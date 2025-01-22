using BaseLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Security;
using Server.Services.Interfaces;

namespace Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string?> RegisterAsync(RegisterRequestDTO request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return null;

            var hashedPassword = PBKDF2Hasher.HashPassword(request.Password);
            var secretKey = TwoFactorAuthService.GenerateSecretKey();

            var newUser = new BaseLibrary.Entities.User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                TwoFactorSecret = secretKey
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return TwoFactorAuthService.GetQrCodeUri(request.Username, secretKey);
        }

        public async Task<bool> ValidateUserAsync(LoginRequestDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                return false;

            return PBKDF2Hasher.VerifyPassword(request.Password, user.PasswordHash);
        }

        public async Task<bool> ValidateTwoFactorCodeAsync(string username, string code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return false;

            return TwoFactorAuthService.ValidateTOTP(user.TwoFactorSecret, code);
        }

        public async Task<string> GetTwoFactorSecretAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user?.TwoFactorSecret ?? string.Empty;
        }
    }
}
