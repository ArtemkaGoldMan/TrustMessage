using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Security;
using Server.Services.Interfaces;
using System;

namespace Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UpdatePasswordAsync(string username, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return false;

            user.PasswordHash = PBKDF2Hasher.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
