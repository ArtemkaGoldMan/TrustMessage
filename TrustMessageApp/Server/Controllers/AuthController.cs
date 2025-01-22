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
            var qrCodeUri = await _authService.RegisterAsync(request);
            if (qrCodeUri == null)
                return BadRequest("Username already exists");

            return Ok(new { message = "Registration successful", qrCodeUri });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!await _authService.ValidateUserAsync(request))
                return Unauthorized("Invalid credentials");

            if (!await _authService.ValidateTwoFactorCodeAsync(request.Username, request.TwoFactorCode))
                return Unauthorized("Invalid 2FA code");

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            if (!await _authService.ValidateTwoFactorCodeAsync(request.Username, request.TwoFactorCode))
                return Unauthorized("Invalid 2FA code");

            if (!await _userService.UpdatePasswordAsync(request.Username, request.NewPassword))
                return NotFound("User not found");

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
