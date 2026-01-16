using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace SEM_Drahos.Data.entities;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}