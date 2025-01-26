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
                // Set the authentication cookie
                var cookies = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
                if (!string.IsNullOrEmpty(cookies))
                {
                    _httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                }

                await _authStateProvider.SetAuthenticatedUser(loginRequest.Username);
                return true;
            }

            return false;
        }

        public async Task<string?> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", registerRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RegisterResponseDTO>();
                return result?.QrCodeUri; // Return the QR code URI
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.SetUnauthenticatedUser();
        }

    }

    public class RegisterResponseDTO
    {
        public string Message { get; set; }
        public string QrCodeUri { get; set; }
    }
}
