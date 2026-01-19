using System.Net.Http.Json;
using FE3.Api;

public sealed class AuthService
{
    private readonly AuthApi _api;
    private readonly TokenStore _tokens;

    public AuthService(AuthApi api, TokenStore tokens)
    {
        _api = api;
        _tokens = tokens;
    }

    public async Task<bool> Login(string username, string password)
    {
        var result = await _api.LoginAsync(username, password);

        if (result == null)
            return false;

        await _tokens.Set(result.Token);
        return true;
    }

    public async Task<bool> Register(string username, string password)
        => await _api.RegisterAsync(username, password);

    public async Task Logout()
        => await _tokens.Clear();
}


public record LoginResponse(string Token);