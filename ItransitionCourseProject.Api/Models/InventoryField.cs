namespace ItransitionCourseProject.Api.Models;

public sealed class InventoryField
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public Type Type { get; set; }
    public bool IsDisplayed { get; set; }
        
    public Guid InventoryId { get; set; }
    public required Inventory Inventory { get; set; }
}

public enum Type
{
    Text,
    MultilineText,
    Number,
    Link,
    Boolean
}