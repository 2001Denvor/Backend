// Models/User.cs
namespace MyBackend.Models
{
    public class User
    {
        public string? Email { get; set; }
        public string? Password { get; set; }  // Plain text for simplicity; hash in production
        public string? Role { get; set; }
    }
}
