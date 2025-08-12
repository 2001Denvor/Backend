using Microsoft.AspNetCore.Identity;
using System;

namespace MyBackend.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "user";

        // New column to track signup date/time
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
