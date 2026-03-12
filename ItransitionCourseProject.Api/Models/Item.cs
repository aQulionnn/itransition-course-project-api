using ItransitionCourseProject.Api.Primitives;

namespace ItransitionCourseProject.Api.Models;

public sealed class Item : IAuditableEntity
{
    public Guid Id { get; set; }
    public required string CustomId { get; set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public Guid InventoryId { get; set; }
    public required Inventory Inventory { get; set; }

    public required string CreatorId { get; set; }
    public required AppUser Creator { get; set; }

    public IEnumerable<ItemFieldValue> ItemFieldValues { get; set; } = [];
    public IEnumerable<ItemLike> ItemLikes { get; set; } = [];
}