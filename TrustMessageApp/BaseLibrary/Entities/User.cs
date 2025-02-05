using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty; // RSA Public Key
        public string PrivateKey { get; set; } = string.Empty; // RSA Private Key
        public string TwoFactorSecret { get; set; } = string.Empty; // Store secret
        public int FailedLoginAttempts { get; set; } = 0; // Track failed login attempts
        public DateTime? LockoutEnd { get; set; } // Track lockout time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
