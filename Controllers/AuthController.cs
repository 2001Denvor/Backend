using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyBackend.DTOs;
using MyBackend.Models;
using MyBackend.Services;
using Microsoft.Extensions.Configuration;

namespace MyBackend.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<User> userManager,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        // ✅ Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new { error = "Email is required." });

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { error = "User already exists." });

            if (string.IsNullOrEmpty(model.Password))
                return BadRequest(new { error = "Password is required." });

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email, // ✅ Important: Identity requires this
                Role = model.Role
            };


            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new { error = "User creation failed.", details = result.Errors });

            return Ok(new { message = "User registered successfully!" });
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return Unauthorized(new { error = "Email and password are required." });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { error = "Invalid credentials." });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized(new { error = "Invalid credentials." });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.Role
                }
            });
        }

        // ✅ Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new { error = "Email is required." });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Ok(new { message = "If the email is valid, a reset link has been sent." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var emailEscaped = Uri.EscapeDataString(user.Email ?? string.Empty);
            var tokenEscaped = Uri.EscapeDataString(token);
            var resetLink = $"http://localhost:5173/reset-password?email={emailEscaped}&token={tokenEscaped}";

            await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Reset Password",
                $"<p>Click the link below to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>");

            return Ok(new { message = "Password reset link sent to your email." });
        }

        // ✅ Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest(new { error = "Email, token, and new password are required." });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { error = "Invalid request." });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new { error = "Failed to reset password.", details = result.Errors });

            return Ok(new { message = "Password has been reset successfully." });
        }

        // ✅ Helper to generate JWT
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                new Claim("id", user.Id?.ToString() ?? string.Empty),
                new Claim("role", user.Role ?? "user")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
