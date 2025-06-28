using Microsoft.AspNetCore.Mvc;                  // ✅ Needed for ControllerBase, IActionResult, [HttpPost], etc.
using MyBackend.Models;                         // ✅ Adjust if your LoginRequest is elsewhere
using MyBackend.Services;                       // ✅ Adjust for your AuthService
using System;

namespace MyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { error = "Email and password are required." });
                }

                var user = _authService.Authenticate(request.Email, request.Password);
                if (user == null)
                {
                    return Unauthorized(new { error = "Invalid credentials" });
                }

                return Ok(new { email = user.Email, role = user.Role });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login exception: " + ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
