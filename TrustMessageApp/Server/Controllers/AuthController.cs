using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var qrCodeUri = await _authService.RegisterAsync(request);
            if (qrCodeUri == null)
                return BadRequest("Registration failed");

            return Ok(new { message = "Registration successful", qrCodeUri });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                bool userValid = await _authService.ValidateUserAsync(request);
                bool twoFactorValid = await _authService.ValidateTwoFactorCodeAsync(request.Username, request.TwoFactorCode);

                if (!userValid || !twoFactorValid)
                {
                    return Unauthorized(new { message = "Invalid login attempt" });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { 
                    message = "Login successful",
                    username = request.Username
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("locked"))
                {
                    return Unauthorized(new { message = "Your account is locked. Please try again later." });
                }
                return Unauthorized(new { message = "Invalid login attempt" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Destroy the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new { 
                    isAuthenticated = true, 
                    username = User.Identity.Name 
                });
            }
            return Unauthorized(new { 
                isAuthenticated = false,
                message = "Not authenticated"
            });
        }
    }
}