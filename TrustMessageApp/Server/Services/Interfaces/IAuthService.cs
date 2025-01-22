using BaseLibrary.DTOs;

namespace Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterRequestDTO request);
        Task<bool> ValidateUserAsync(LoginRequestDTO request);
        Task<bool> ValidateTwoFactorCodeAsync(string username, string code);
        Task<string> GetTwoFactorSecretAsync(string username);
    }
}
