using Microsoft.AspNetCore.Identity;
using MyBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBackend.Services
{
    public class AuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        private List<User> _users;

        public AuthService()
        {
            _passwordHasher = new PasswordHasher<User>();

            // Dummy users with hashed passwords
            var admin = new User { Email = "admin@example.com", Role = "admin" };
            admin.PasswordHash = _passwordHasher.HashPassword(admin, "admin123");

            var user = new User { Email = "user@example.com", Role = "user" };
            user.PasswordHash = _passwordHasher.HashPassword(user, "user123");

            _users = new List<User> { admin, user };
        }

        public User? Authenticate(string email, string password)
        {
            var user = _users.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            // ✅ Check if PasswordHash is null before verifying
            if (user.PasswordHash == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Success)
                return user;

            return null;
        }
    }
}
