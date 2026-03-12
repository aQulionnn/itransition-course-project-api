namespace ItransitionCourseProject.Api.Models;

public sealed class ItemFieldValue
{
    public Guid Id { get; set; }
    public required string Value { get; set; }

    public Guid ItemId { get; set; }
    public required Item Item { get; set; }
    
    public Guid InventoryFieldId { get; set; }
    public required InventoryField InventoryField { get; set; }
}