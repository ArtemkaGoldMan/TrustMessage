using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public Task SetAuthenticatedUser(string username)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "apiauth");

        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        // Set the authentication cookie
        _httpClient.DefaultRequestHeaders.Add("Cookie", "HttpOnly; Secure");

        return Task.CompletedTask;
    }

    public Task SetUnauthenticatedUser()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        // Clear the authentication cookie
        _httpClient.DefaultRequestHeaders.Remove("Cookie");

        return Task.CompletedTask;
    }
}