namespace ItransitionCourseProject.Api.Models;

public sealed class Category
{
    public Guid Id { get; init; }
    public required string Name { get; set; }

    public IEnumerable<Inventory> Inventories { get; set; } = [];
}