using BaseLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Security;
using Server.Services.Interfaces;
using System;

namespace Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> RegisterAsync(RegisterRequestDTO request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return null;

            // Generate RSA key pair using KeyManager
            var (publicKey, privateKey) = KeyManager.GenerateRsaKeyPair();

            // Encrypt the private key using the user's password
            string encryptedPrivateKey = KeyManager.EncryptPrivateKey(privateKey, request.Password);

            var hashedPassword = PBKDF2Hasher.HashPassword(request.Password);
            var secretKey = TwoFactorAuthService.GenerateSecretKey();

            var newUser = new BaseLibrary.Entities.User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                PublicKey = publicKey,
                PrivateKey = encryptedPrivateKey, // Store the encrypted private key
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

            // Check if the user is currently locked out
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new Exception($"Your account is locked until {user.LockoutEnd.Value.ToLocalTime()}. Please try again later.");
            }

            // If the lockout period has expired, reset the lockout
            if (user.LockoutEnd.HasValue && user.LockoutEnd <= DateTime.UtcNow)
            {
                user.LockoutEnd = null;
                user.FailedLoginAttempts = 0;
                await _context.SaveChangesAsync();
            }

            if (!PBKDF2Hasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15); // Lock the user for 15 minutes
                    await _context.SaveChangesAsync();
                    throw new Exception("Your account has been locked due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(2)); // Small delay to mitigate brute-force attacks
                }

                await _context.SaveChangesAsync();
                return false;
            }

            // Successful login - reset failed attempts
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
