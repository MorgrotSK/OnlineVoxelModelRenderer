using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SEM_Drahos.Data;
using SEM_Drahos.Data.entities;

namespace SEM_Drahos.utils;

public class UserHelpers
{
    public static async Task<(bool ok, User? user, IResult? error)> ValidateUserAsync(ClaimsPrincipal principal, PotDbContext db)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return (false, null, Results.Unauthorized());

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return (false, null, Results.Unauthorized());

        return (true, user, null);
    }
}