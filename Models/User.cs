using Microsoft.AspNetCore.Identity;


namespace MyBackend.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } ="";
        public string Role { get; set; } = "user";
    }
}
