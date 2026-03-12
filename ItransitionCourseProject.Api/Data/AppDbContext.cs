using ItransitionCourseProject.Api.Data.Configurations;
using ItransitionCourseProject.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItransitionCourseProject.Api.Data;

public class AppDbContext(DbContextOptions options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<InventoryTag> InventoryTags { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<InventoryAccess> InventoryAccesses { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<InventoryField> InventoryFields { get; set; }
    public DbSet<ItemFieldValue> ItemFieldValues { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<ItemLike> ItemLikes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfiguration(new IdentityRoleConfiguration());
        
        builder.Entity<Inventory>()
            .HasIndex(x => new { x.Title, x.Description })
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");

        builder.Entity<ItemFieldValue>()
            .HasIndex(x => x.Value)
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");

        builder.Entity<Post>()
            .HasIndex(x => x.Content)
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");
    }
}