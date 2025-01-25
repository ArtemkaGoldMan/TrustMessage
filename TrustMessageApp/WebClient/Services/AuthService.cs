using BaseLibrary.DTOs;
using System.Net.Http.Json;

namespace WebClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, CustomAuthStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequestDTO loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                await _authStateProvider.SetAuthenticatedUser(loginRequest.Username);
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", registerRequest);
            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.SetUnauthenticatedUser();
        }
    }
}
