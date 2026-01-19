using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SEM_Drahos.Data;
using SEM_Drahos.Data.entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace SEM_Drahos.Endpoints;

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string UserName, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapGet("/me", Me).RequireAuthorization();
    }

    private static async Task<IResult> Register(PotDbContext db, RegisterRequest req)
    {
        if (await db.Users.AnyAsync(u => u.UserName == req.Username))
            return Results.BadRequest("User already exists");

        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            UserName = req.Username,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = hasher.HashPassword(user, req.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Results.Ok();
    }


    private static async Task<IResult> Login(PotDbContext db, IConfiguration config, LoginRequest req) {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == req.UserName);
        if (user == null)
            return Results.Unauthorized();

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);

        if (result == PasswordVerificationResult.Failed)
            return Results.Unauthorized();

        var jwt = config.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return Results.Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }


    private static IResult Me(ClaimsPrincipal user)
    {
        return Results.Ok(new
        {
            Id = user.FindFirstValue(JwtRegisteredClaimNames.Sub),
            UserName = user.FindFirstValue(JwtRegisteredClaimNames.Name)
        });
    }
}
