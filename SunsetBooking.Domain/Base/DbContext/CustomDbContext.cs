using Microsoft.EntityFrameworkCore;

namespace SunsetBooking.Domain.Base.DbContext;

public class CustomDbContext<T> : Microsoft.EntityFrameworkCore.DbContext where T : Microsoft.EntityFrameworkCore.DbContext
{
    protected readonly IUserContext _userContext;

    public CustomDbContext() : base()
    {
    }

    public CustomDbContext(IUserContext userContext) : base()
    {
        _userContext = userContext;
    }

    public CustomDbContext(DbContextOptions<T> options, IUserContext userContext) : base(options)
    {
        _userContext = userContext;
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleAudit();
        HandleDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        HandleAudit();
        HandleDelete();
        return base.SaveChanges();
    }

    private void HandleAudit()
    {
        var changeSet = ChangeTracker.Entries<IAuditable>();

        if (changeSet != null)
        {
            var currentTime = DateTime.UtcNow;
            foreach (var entry in changeSet.Where(c => c.State == EntityState.Added))
            {
                entry.Entity.CreatedAt = currentTime;
                entry.Entity.CreatedById = _userContext.UserId;
            }

            foreach (var entry in changeSet.Where(c => c.State == EntityState.Modified))
            {
                entry.Entity.ModifiedAt = currentTime;
                entry.Entity.ModifiedById = _userContext.UserId;
            }
        }
    }

    private void HandleDelete()
    {
        var changeSet = ChangeTracker.Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);
        if (changeSet != null)
        {
            foreach (var entry in changeSet)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
            }
        }
    }
}