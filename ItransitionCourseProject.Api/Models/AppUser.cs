using Microsoft.AspNetCore.Identity;

namespace ItransitionCourseProject.Api.Models;

public sealed class AppUser : IdentityUser
{
    public bool IsBlocked { get; set; }
    
    public IEnumerable<Inventory> Inventories { get; init; } = [];
    public IEnumerable<InventoryAccess> InventoriesAccess { get; init; } = [];
    public IEnumerable<Item> Items { get; init; } = [];
    public IEnumerable<Post>  Posts { get; init; } = [];
    public IEnumerable<ItemLike> ItemLikes { get; init; } = [];
}