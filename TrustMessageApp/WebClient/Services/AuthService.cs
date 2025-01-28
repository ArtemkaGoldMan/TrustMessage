using BaseLibrary.DTOs;
using System.Net.Http.Json;

namespace WebClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RegisterAsync(RegisterRequestDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                return result?.QrCodeUri;
            }
            return null;
        }

        public async Task<bool> LoginAsync(LoginRequestDTO request)
        {
            var response = await _httpClient.GetAsync(
                $"api/auth/login?username={request.Username}&password={request.Password}&twoFactorCode={request.TwoFactorCode}");
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var response = await _httpClient.GetAsync("api/auth/check");
            return response.IsSuccessStatusCode;
        }

        private class RegisterResponse
        {
            public string QrCodeUri { get; set; }
        }
    }
}
