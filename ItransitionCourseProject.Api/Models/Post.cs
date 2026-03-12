using ItransitionCourseProject.Api.Primitives;

namespace ItransitionCourseProject.Api.Models;

public sealed class Post : IAuditableEntity
{
    public Guid Id { get; set; }
    public required string Content { get; set; } 
    
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    
    public required string UserId { get; set; }
    public required AppUser User { get; set; }
    
    public Guid InventoryId { get; set; }
    public required Inventory Inventory { get; set; }
}