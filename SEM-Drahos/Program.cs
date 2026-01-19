using MongoDB.Bson;
using SEM_Drahos.Data;
using SEM_Drahos.Data.entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SEM_Drahos.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection("Jwt");
/* ---------- AUTH ---------- */
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();

/* ---------- CORS (REQUIRED FOR WASM) ---------- */
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

/* ---------- SERVICES ---------- */
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PotDbContext>();

var app = builder.Build();

/* ---------- REQUEST LOGGING ---------- */
app.Use(async (context, next) =>
{
    var request = context.Request;

    Console.WriteLine($"[{DateTime.UtcNow:O}] {request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString}");

    await next();
});

/* ---------- PIPELINE ---------- */
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

/* ---------- ENDPOINTS ---------- */
app.MapAuthEndpoints();
app.MapModelEndpoints();

app.Run();
