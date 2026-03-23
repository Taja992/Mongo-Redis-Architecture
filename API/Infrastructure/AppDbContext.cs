using API.Core.Domain.Entities;
// using API.Core.Domain.Context;
// using API.Core.Domain.Entities.Interfaces;
// using API.Core.Domain.Identity.Entities;
// using API.Infrastructure.Util;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Infrastructure;

// Uncomment this when identity types are added:
// public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
public class AppDbContext : DbContext
{
    private readonly TimeProvider _timeProvider;

    // private readonly ICurrentContext _currentContext;

    // Uncomment this constructor when context interfaces are available:
    // public AppDbContext(
    //     DbContextOptions<AppDbContext> options,
    //     TimeProvider timeProvider,
    //     ICurrentContext currentContext
    // )
    //     : base(options)
    // {
    //     _timeProvider = timeProvider;
    //     _currentContext = currentContext;
    //
    //     ChangeTracker.Tracked += OnEntityTracked;
    //     ChangeTracker.StateChanged += OnEntityStateChanged;
    // }

    public AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider timeProvider)
        : base(options)
    {
        _timeProvider = timeProvider;

        ChangeTracker.Tracked += OnEntityTracked;
        ChangeTracker.StateChanged += OnEntityStateChanged;
    }

    // public DbSet<AppUser> AppUsers { get; set; } = null!;
    // public DbSet<OAuthAccount> OAuthAccounts { get; set; } = null!;
    public DbSet<MongoRedisArchitecture> Tests { get; set; } = null!;

    // public DbSet<StripeAccount> StripeAccounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically apply IEntityTypeConfiguration<T> from this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Uncomment once IOwned/ISoftDeletable/current user context are added.
        // modelBuilder
        //     .Model.GetEntityTypes()
        //     .ToList()
        //     .ForEach(entityType =>
        //     {
        //         if (typeof(IOwned).IsAssignableFrom(entityType.ClrType))
        //         {
        //             modelBuilder
        //                 .Entity(entityType.ClrType)
        //                 .AddQueryFilter<IOwned>(e => e.AppUserId == _currentContext.UserId);
        //         }
        //
        //         if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
        //         {
        //             modelBuilder
        //                 .Entity(entityType.ClrType)
        //                 .AddQueryFilter<ISoftDeletable>(e => !e.IsDeleted);
        //         }
        //     });
    }

    private void OnEntityTracked(object? sender, EntityTrackedEventArgs e)
    {
        // Strongly-typed target pattern:
        // if (e.Entry.State is EntityState.Added && e.Entry.Entity is ITrackable trackableEntity)
        // {
        //     var now = _timeProvider.GetUtcNow();
        //     trackableEntity.CreatedAt = now;
        //     trackableEntity.UpdatedAt = now;
        // }

        if (e.Entry.State is not EntityState.Added)
        {
            return;
        }

        SetAuditPropertyIfExists(e.Entry.Entity, "CreatedAt", _timeProvider.GetUtcNow());
        SetAuditPropertyIfExists(e.Entry.Entity, "UpdatedAt", _timeProvider.GetUtcNow());
    }

    private void OnEntityStateChanged(object? sender, EntityStateChangedEventArgs e)
    {
        // Strongly-typed target pattern:
        // if (e.NewState is EntityState.Modified && e.Entry.Entity is ITrackable trackableEntity)
        // {
        //     trackableEntity.UpdatedAt = _timeProvider.GetUtcNow();
        // }

        if (e.NewState is not EntityState.Modified)
        {
            return;
        }

        SetAuditPropertyIfExists(e.Entry.Entity, "UpdatedAt", _timeProvider.GetUtcNow());
    }

    private static void SetAuditPropertyIfExists(
        object entity,
        string propertyName,
        DateTimeOffset value
    )
    {
        var property = entity.GetType().GetProperty(propertyName);

        if (property?.CanWrite is not true)
        {
            return;
        }

        if (property.PropertyType == typeof(DateTimeOffset))
        {
            property.SetValue(entity, value);
            return;
        }

        if (property.PropertyType == typeof(DateTimeOffset?))
        {
            property.SetValue(entity, (DateTimeOffset?)value);
        }
    }

    public override void Dispose()
    {
        ChangeTracker.Tracked -= OnEntityTracked;
        ChangeTracker.StateChanged -= OnEntityStateChanged;
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    public override async ValueTask DisposeAsync()
    {
        ChangeTracker.Tracked -= OnEntityTracked;
        ChangeTracker.StateChanged -= OnEntityStateChanged;
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
