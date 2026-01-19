using SEM_Drahos.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SEM_Drahos.Data.entities;
using SEM_Drahos.utils;


namespace SEM_Drahos.Endpoints;

public static class VoxelModelsEndpoints
{
    public static void MapModelEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/models");

        group.MapPost("/upload", UploadModel).RequireAuthorization().DisableAntiforgery();;;
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

        if (!isUserValid)
            return authError!;
        
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
        return Results.Ok(new
        {
            Id = model.Id,
            Name = model.Name,
            CreatedAtUtc = model.CreatedAtUtc
        });

    }
}
