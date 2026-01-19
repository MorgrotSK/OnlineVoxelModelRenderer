using Microsoft.JSInterop;

public class TokenStore
{
    private const string Key = "auth_token";
    private readonly IJSRuntime _js;

    public TokenStore(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> Get()
    {
        // During prerender, JSRuntime is not ready
        if (_js is IJSInProcessRuntime == false &&
            _js.GetType().Name.Contains("RemoteJSRuntime"))
        {
            return null;
        }

        try
        {
            return await _js.InvokeAsync<string?>(
                "localStorage.getItem", Key);
        }
        catch
        {
            // Happens during prerender
            return null;
        }
    }

    public async Task Set(string token)
    {
        await _js.InvokeVoidAsync(
            "localStorage.setItem", Key, token);
    }

    public async Task Clear()
    {
        await _js.InvokeVoidAsync(
            "localStorage.removeItem", Key);
    }
}