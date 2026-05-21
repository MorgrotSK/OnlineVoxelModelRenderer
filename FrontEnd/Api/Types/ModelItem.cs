namespace FE3.Api.Types;

public sealed class ModelItem
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public bool IsPrivate { get; set; }
    public string OwnerId { get; set; } = null!;
}