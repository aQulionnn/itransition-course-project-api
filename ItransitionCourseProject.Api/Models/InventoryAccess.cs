namespace ItransitionCourseProject.Api.Models;

public sealed class InventoryAccess
{
    public Guid Id { get; init; }
    public bool HasWritePermission { get; set; }
    
    public Guid InventoryId { get; set; }
    public required Inventory Inventory { get; set; }

    public required string UserId { get; set; }
    public required AppUser User { get; set; }
}