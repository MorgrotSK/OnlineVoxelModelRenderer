using System.Numerics;
using System.Security.Claims;
using SEM_Drahos.Data;
using SEM_Drahos.utils;
using SharedClass;

namespace SEM_Drahos.Endpoints;

public static class WorldEndpoints
{
    public static void MapWorldEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/world");
        group.MapGet("/{worldId}/{u:int}/{v:int}", GetChunk).AllowAnonymous().DisableAntiforgery();
    }
    
    private static async Task<IResult> GetChunk(ClaimsPrincipal user, PotDbContext db, IWebHostEnvironment env, int u, int v, string worldId)
    {
        string fileName = "{" + u + "}" + "{" + v + "}" + ".fotr";
        string worldDir = Path.Combine(env.ContentRootPath, "world", worldId);
        

        var path = Path.Combine(env.ContentRootPath, worldDir, fileName);

        if (!File.Exists(path))
        {
            // Ensure world directory exists
            Directory.CreateDirectory(worldDir);

            // Generate chunk if missing
            if (!File.Exists(path))
            {
                var generator = new ChunkGenerator();

                // NOTE: u,v are chunk coordinates, not block coordinates
                var chunk = generator.GenerateChunk(new Vector2(u, v));

                byte[] data = chunk.Serialize();
                chunk.Dispose();

                await File.WriteAllBytesAsync(path, data);
            }

        }
        
        return Results.File(
            path,
            contentType: "application/octet-stream",
            enableRangeProcessing: true,
            lastModified: File.GetLastWriteTimeUtc(path));
    }
}