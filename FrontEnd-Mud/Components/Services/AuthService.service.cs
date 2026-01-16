public class AuthService
{
    private readonly HttpClient _http;
    private readonly TokenStore _tokens;

    public AuthService(HttpClient http, TokenStore tokens)
    {
        _http = http;
        _tokens = tokens;
    }

    public async Task<bool> Login(string userName, string password)
    {
        Console.WriteLine($"Login: {userName}");
        var res = await _http.PostAsJsonAsync("/auth/login", new
        {
            userName,
            password
        });

        if (!res.IsSuccessStatusCode)
            return false;

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
        await _tokens.Set(data!.Token);
        return true;
    }

    public async Task<bool> Register(string username, string password)
    {
        var res = await _http.PostAsJsonAsync("/auth/register", new
        {
            username,
            password
        });

        return res.IsSuccessStatusCode;
    }

    public async Task Logout()
        => await _tokens.Clear();
}

public record LoginResponse(string Token);