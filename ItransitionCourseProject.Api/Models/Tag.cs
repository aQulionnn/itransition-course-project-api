namespace ItransitionCourseProject.Api.Models;

public sealed class Tag
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public ICollection<InventoryTag> InventoryTags { get; set; } = [];
}