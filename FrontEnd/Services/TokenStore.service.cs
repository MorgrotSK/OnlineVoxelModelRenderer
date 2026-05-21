using Microsoft.JSInterop;

namespace FrontEnd.Services;

public class TokenStore(IJSRuntime js)
{
    private const string Key = "auth_token";

    public async Task<string?> Get()
    {
        // During prerender, JSRuntime is not ready
        if (js is IJSInProcessRuntime == false &&
            js.GetType().Name.Contains("RemoteJSRuntime"))
        {
            return null;
        }

        try
        {
            return await js.InvokeAsync<string?>(
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
        await js.InvokeVoidAsync(
            "localStorage.setItem", Key, token);
    }

    public async Task Clear()
    {
        await js.InvokeVoidAsync(
            "localStorage.removeItem", Key);
    }
}