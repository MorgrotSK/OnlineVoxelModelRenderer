using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStore _tokens;

    public JwtAuthStateProvider(TokenStore tokens)
    {
        _tokens = tokens;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokens.Get();
        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void Notify() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}