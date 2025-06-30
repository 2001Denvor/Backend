using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBackend.Models
{
    public class RegisterRequest
    {
        [Required]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = "user"; // ✅ Default to "user", but can be passed from frontend
    }
}
