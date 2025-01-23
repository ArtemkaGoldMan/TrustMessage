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

            // Check if user is locked out
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new Exception("Your account is locked. Try again later.");
            }

            if (!PBKDF2Hasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    await _context.SaveChangesAsync();
                    throw new Exception("Your account has been locked due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1)); // Small delay
                }

                await _context.SaveChangesAsync();
                return false;
            }

            // Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return true;
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
