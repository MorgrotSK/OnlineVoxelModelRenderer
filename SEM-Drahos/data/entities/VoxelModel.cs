using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SEM_Drahos.Data.entities;

public sealed class VoxelModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}