using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;

namespace Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
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
                    return Unauthorized("Invalid login attempt");
                }

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

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool twoFactorValid = await _authService.ValidateTwoFactorCodeAsync(request.Username, request.TwoFactorCode);
            if (!twoFactorValid)
                return Unauthorized("Invalid request");

            bool passwordChanged = await _userService.UpdatePasswordAsync(request.Username, request.NewPassword);
            if (!passwordChanged)
                return NotFound("User not found");

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
