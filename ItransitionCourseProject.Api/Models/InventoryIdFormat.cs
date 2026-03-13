namespace ItransitionCourseProject.Api.Models;

public sealed class InventoryIdFormat
{
    public Guid Id { get; set; }
    public Guid InventoryId { get; set; }
    public required Inventory Inventory { get; set; }
    public List<InventoryIdElement> Elements { get; set; }
}