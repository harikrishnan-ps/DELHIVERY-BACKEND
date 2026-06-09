using Microsoft.EntityFrameworkCore;
using Delhivery.Domain.Entities;

namespace Delhivery.Infrastructure.Persistence;

public class DelhiveryDbContext : DbContext
{
    public DelhiveryDbContext(DbContextOptions<DelhiveryDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<FileMetadata> Files { get; set; } = null!;
    public DbSet<OTPLog> OTPLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.PickupAddress)
                  .WithMany()
                  .HasForeignKey(o => o.PickupAddressId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.DeliveryAddress)
                  .WithMany()
                  .HasForeignKey(o => o.DeliveryAddressId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.Property(o => o.Amount).HasColumnType("decimal(18,2)");
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
