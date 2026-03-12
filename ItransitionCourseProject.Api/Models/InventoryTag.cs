namespace ItransitionCourseProject.Api.Models;

public sealed class InventoryTag
{
    public Guid Id { get; init; }
    
    public Guid InventoryId { get; init; }
    public required Inventory Inventory { get; init; }
    
    public Guid TagId { get; init; }
    public required Tag Tag { get; init; }
}