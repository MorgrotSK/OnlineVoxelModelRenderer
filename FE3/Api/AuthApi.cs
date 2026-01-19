namespace FE3.Api;

using System.Net.Http.Json;

public sealed class AuthApi
{
    private readonly HttpClient _api;

    public AuthApi(HttpClient api)
    {
        _api = api;
    }

    public async Task<LoginResponse?> LoginAsync(
        string username,
        string password,
        CancellationToken ct = default)
    {
        var res = await _api.PostAsJsonAsync("/auth/login", new
        {
            userName = username,
            password
        }, ct);

        if (!res.IsSuccessStatusCode)
            return null;

        return await res.Content.ReadFromJsonAsync<LoginResponse>(ct);
    }

    public async Task<bool> RegisterAsync(
        string username,
        string password,
        CancellationToken ct = default)
    {
        var res = await _api.PostAsJsonAsync("/auth/register", new
        {
            username,
            password
        }, ct);

        return res.IsSuccessStatusCode;
    }
}

public record LoginResponse(string Token);
