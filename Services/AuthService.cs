using Microsoft.AspNetCore.Identity;
using MyBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBackend.Services
{
    public interface IAuthService
    {
        User? Authenticate(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly List<User> _users;

        public AuthService()
        {
            _passwordHasher = new PasswordHasher<User>();

            var admin = new User { Email = "admin@example.com", Role = "admin" };
            admin.PasswordHash = _passwordHasher.HashPassword(admin, "admin123");

            var user = new User { Email = "user@example.com", Role = "user" };
            user.PasswordHash = _passwordHasher.HashPassword(user, "user123");

            _users = new List<User> { admin, user };
        }

        public User? Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = _users.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return verificationResult == PasswordVerificationResult.Success ? user : null;
        }
    }
}
