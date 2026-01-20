using SEM_Drahos.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using SEM_Drahos.Data.entities;
using SEM_Drahos.utils;


namespace SEM_Drahos.Endpoints;

public static class VoxelModelsEndpoints
{
    public static void MapModelEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/models");

        group.MapPost("/upload", UploadModel).RequireAuthorization().DisableAntiforgery();
        group.MapGet("/{id}/thumbnail", GetModelThumbnail).AllowAnonymous().DisableAntiforgery();
        group.MapGet("/", GetAllModels).AllowAnonymous().DisableAntiforgery();
        group.MapGet("/{id}", GetModel).AllowAnonymous().DisableAntiforgery();
        group.MapPatch("/{id}/{access:bool}", SetAccess).RequireAuthorization().DisableAntiforgery();
        group.MapDelete("/{id}", RemoveModel).RequireAuthorization().DisableAntiforgery();
        group.MapGet("/{id}/meta", GetModelMeta).AllowAnonymous().DisableAntiforgery();
    }
    
    private static async Task<IResult> GetModel(ClaimsPrincipal user, PotDbContext db, IWebHostEnvironment env, string id)
    {
        var (ok, result, model) = await ModelsHelpers.VerifyModelPermission(user, db, id);

        if (!ok) return result;

        var (ownerId, _, _, createdAtUtc) = model!.Value;

        var path = Path.Combine(env.ContentRootPath, "uploads", ownerId, id, "model.fotr");

        if (!File.Exists(path)) return Results.NotFound();

        return Results.File(
            path,
            contentType: "application/octet-stream",
            enableRangeProcessing: true,
            lastModified: File.GetLastWriteTimeUtc(path));

    }
    
    
    private static async Task<IResult> SetAccess(ClaimsPrincipal user, PotDbContext db, string id, bool access)
    {
        var (ok, result, model) = await ModelsHelpers.VerifyModelPermission(user, db, id);

        if (!ok) return result;
        
        var entity = await db.VoxelModels.FirstOrDefaultAsync(m => m.Id == id);
        entity.IsPrivate = access;
        await db.SaveChangesAsync();

        return Results.Ok(new { id});
    }
    
    
    private static async Task<IResult> RemoveModel(ClaimsPrincipal user, PotDbContext db, string id)
    {
        var (ok, result, model) = await ModelsHelpers.VerifyModelPermission(user, db, id);

        if (!ok) return result;
        
        db.VoxelModels.Remove(await db.VoxelModels.FirstOrDefaultAsync(m => m.Id == id));
        await db.SaveChangesAsync();

        return Results.Ok(new { id});
    }
    
    private static async Task<IResult> GetModelMeta(ClaimsPrincipal user, PotDbContext db, string id)
    {
        var (ok, result, model) = await ModelsHelpers.VerifyModelPermission(user, db, id);

        if (!ok) return result;

        var (ownerId, isPrivate, name, createdAtUtc) = model!.Value;

        return Results.Ok(new { id = id, name = name, createdAtUtc = createdAtUtc, isPrivate = isPrivate, ownerId = ownerId });
    }



    private static async Task<IResult> GetModelThumbnail(ClaimsPrincipal user, PotDbContext db, IWebHostEnvironment env, string id)
    {
        var (ok, result, model) = await ModelsHelpers.VerifyModelPermission(user, db, id);
        
        var path = Path.Combine(env.ContentRootPath, "uploads", model.Value.OwnerId, id, "thumbnail.png");

        if (!File.Exists(path))
            return Results.NotFound();

        return Results.File(path, "image/png", enableRangeProcessing: true);
    }

    private static async Task<IResult> GetAllModels(ClaimsPrincipal user, PotDbContext db)
    {
        var models = await db.VoxelModels
            .Where(m => !m.IsPrivate && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAtUtc)
            .ToListAsync();

        return Results.Ok(models);
    }

    private static async Task<IResult> UploadModel(
        IFormFile voxelModel, 
        IFormFile thumbnail, 
        [FromForm] string name,
        ClaimsPrincipal user, 
        PotDbContext db)
    {
        //Validate user
        
        var (isUserValid, dbUser, authError) = await UserHelpers.ValidateUserAsync(user, db);

        if (!isUserValid) return authError!;
        
        //Data validation
        if (string.IsNullOrWhiteSpace(name))
            return Results.BadRequest("File name is required");
        
        var (isModelValid, modelError) = await ModelsHelpers.ValidateModelAsync(voxelModel);
        if (!isModelValid)
            return Results.BadRequest(modelError);
        
        var (isThumbnailValid, thumbnailError) = await ModelsHelpers.ValidateModelThumbnailAsync(thumbnail);
        if (!isThumbnailValid)
            return Results.BadRequest(thumbnailError);
        
        //Save files
        var modelId = ObjectId.GenerateNewId().ToString();
        var dir = Path.Combine("uploads", dbUser.Id, modelId);
        Directory.CreateDirectory(dir);

        var modelPath = Path.Combine(dir, "model.fotr");
        var thumbPath = Path.Combine(dir, "thumbnail.png");

        await using (var fs = File.Create(modelPath))
            await voxelModel.CopyToAsync(fs);

        await using (var fs = File.Create(thumbPath))
            await thumbnail.CopyToAsync(fs);
        
        //Save db data
        var model = new VoxelModel
        {
            Id = modelId,
            OwnerId = dbUser.Id!,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.VoxelModels.Add(model);
        await db.SaveChangesAsync();

        // --- Response ---
        return Results.Ok(new { Id = model.Id, Name = model.Name, CreatedAtUtc = model.CreatedAtUtc });

    }
}
