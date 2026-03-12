using ItransitionCourseProject.Api.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ItransitionCourseProject.Api.Interceptors;

public sealed  class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        DbContext? context = eventData.Context;
        
        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        IEnumerable<EntityEntry<IAuditableEntity>> entries = context.ChangeTracker.Entries<IAuditableEntity>();

        foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
                entityEntry.Property(x => x.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            
            if (entityEntry.State == EntityState.Modified)
                entityEntry.Property(x => x.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}