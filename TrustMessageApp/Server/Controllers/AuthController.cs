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

        [HttpGet("login")]
        public async Task<IActionResult> LoginForSwagger([FromQuery] string username, [FromQuery] string password, [FromQuery] string twoFactorCode)
        {
            var request = new LoginRequestDTO
            {
                Username = username,
                Password = password,
                TwoFactorCode = twoFactorCode
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                bool userValid = await _authService.ValidateUserAsync(request);
                bool twoFactorValid = await _authService.ValidateTwoFactorCodeAsync(request.Username, request.TwoFactorCode);

                if (!userValid || !twoFactorValid)
                {
                    return Unauthorized("Invalid login attempt");
                }

                // Create the authentication cookie
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { message = "Login successful" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("locked"))
                {
                    return Unauthorized("Your account is locked. Please try again later.");
                }
                return Unauthorized("Invalid login attempt");
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
            if (User.Identity.IsAuthenticated)
            {
                return Ok();
            }
            return Unauthorized();
        }
    }
}