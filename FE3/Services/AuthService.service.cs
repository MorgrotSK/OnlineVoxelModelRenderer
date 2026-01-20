using FE3.Api;
using Microsoft.AspNetCore.Components.Authorization;

public sealed class AuthService
{
    private readonly AuthApi _api;
    private readonly TokenStore _tokens;
    private readonly JwtAuthStateProvider _auth;

    public AuthService(AuthApi api, TokenStore tokens, AuthenticationStateProvider auth)
    {
        _api = api;
        _tokens = tokens;
        _auth = (JwtAuthStateProvider)auth;
    }

    public async Task<bool> Login(string username, string password)
    {
        var result = await _api.LoginAsync(username, password);
        if (result == null)
            return false;

        await _tokens.Set(result.Token);
        _auth.Notify();

        return true;
    }

    public async Task Logout()
    {  
        await _tokens.Clear();
        _auth.Notify();
    }
    
    public async Task<bool> Register(string username, string password) => await _api.RegisterAsync(username, password);
}