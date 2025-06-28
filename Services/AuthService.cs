using MyBackend.Models; // for LoginRequest or User
using System.Collections.Generic;

namespace MyBackend.Services
{
    public class AuthService
    {
        // Dummy user list — replace with real database access
        private List<User> _users = new List<User>
        {
            new User { Email = "admin@example.com", Password = "admin123", Role = "admin" },
            new User { Email = "user@example.com", Password = "user123", Role = "user" }
        };

        public User? Authenticate(string email, string password)
        {
            // In real apps, hash the password and compare
            return _users.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password
            );

        }
    }
}
