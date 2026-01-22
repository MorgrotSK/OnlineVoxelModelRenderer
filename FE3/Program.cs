using Ab4d.SharpEngine.WebGL.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FE3;
using FE3.Api;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSingleton(new Uri("http://localhost:5201"));

/* ---------- AUTH CORE ---------- */
builder.Services.AddAuthorizationCore();

/* ---------- Token storage (MUST be first) ---------- */
builder.Services.AddScoped<TokenStore>();

/* ---------- Auth plumbing ---------- */
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<AuthApi>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();

/* ---------- HttpClient ---------- */
builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri("http://localhost:5201"); // backend
}).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));
builder.Services.AddScoped<ModelsApi>();
builder.Services.AddScoped<ModelsService>();
builder.Services.AddScoped<WorldApi>();
builder.Services.AddScoped<WorldService>();

await builder.Build().RunAsync();