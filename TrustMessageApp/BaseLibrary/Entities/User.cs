﻿using System;
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
        public string TwoFactorSecret { get; set; } = string.Empty; // Store TOTP/HOTP secret
        public int FailedLoginAttempts { get; set; } = 0; // Track failed login attempts
        public DateTime? LockoutEnd { get; set; } // Track lockout time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
