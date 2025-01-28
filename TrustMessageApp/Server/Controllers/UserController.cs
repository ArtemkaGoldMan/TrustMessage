using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using Server.Security;

namespace Server.Controllers
{
    [Authorize] 
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet("find/{username}")]
        public async Task<IActionResult> FindUser(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Username,
                user.Email
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool twoFactorValid = await _authService.ValidateTwoFactorCodeAsync(request.Username, request.TwoFactorCode);
            if (!twoFactorValid)
                return Unauthorized("Invalid request");

            if (!PasswordStrengthChecker.IsPasswordStrong(request.NewPassword))
                return BadRequest("New password is not strong enough");

            bool passwordChanged = await _userService.UpdatePasswordAsync(request.Username, request.NewPassword);
            if (!passwordChanged)
                return NotFound("User not found");

            return Ok(new { message = "Password changed successfully" });
        }
    }
}