namespace ItransitionCourseProject.Api.Models;

public sealed class ItemLike
{
    public Guid Id { get; set; }
    
    public Guid ItemId { get; set; }
    public required Item Item { get; set; }
    
    public required string UserId { get; set; }
    public required AppUser User { get; set; }
}