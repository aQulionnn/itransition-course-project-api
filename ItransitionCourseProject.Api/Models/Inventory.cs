using ItransitionCourseProject.Api.Primitives;

namespace ItransitionCourseProject.Api.Models;

public sealed class Inventory : IAuditableEntity
{
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsPublic { get; set; }

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; set; }

    public required string CreatorId { get; set; }
    public required AppUser Creator { get; set; }
    
    public Guid CategoryId { get; set; }
    public required Category Category { get; set; }

    public List<InventoryAccess> InventoryAccesses { get; set; } 
    public List<InventoryTag> InventoryTags { get; set; }
    public List<Item> Items { get; set; }
    public List<InventoryField> InventoryFields { get; set; }
    public List<ItemFieldValue> ItemFieldValues { get; set; } 
    public List<Post> Posts { get; set; }
}