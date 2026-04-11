using Microsoft.EntityFrameworkCore;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Interfaces.Base;
using System.Linq.Expressions;

namespace QLSanBong.Infrastructure.Data;

public class QLSanBongDbContext(DbContextOptions<QLSanBongDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Pitch> Pitches { get; set; }
    public DbSet<PitchBooking> PitchBookings { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<BookingService> BookingServices { get; set; }
    public DbSet<PitchMaintenance> PitchMaintenances { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Role).HasConversion<string>();
        });

        // Cấu hình bảng Sân bóng
        modelBuilder.Entity<Pitch>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.PitchType).IsRequired().HasMaxLength(50);
            e.Property(x => x.PricePerHour).HasColumnType("decimal(18,2)");
        });

        // Cấu hình Đặt sân
        modelBuilder.Entity<PitchBooking>(e =>
        {
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");

            e.HasOne(b => b.User).WithMany(u => u.PitchBookings).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(b => b.Pitch).WithMany(p => p.PitchBookings).HasForeignKey(b => b.PitchId).OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình Dịch vụ và Bảo trì (Tránh lỗi Decimal)
        modelBuilder.Entity<Service>(e => e.Property(x => x.Price).HasColumnType("decimal(18,2)"));
        modelBuilder.Entity<BookingService>(e => e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)"));
        modelBuilder.Entity<PitchMaintenance>(e => e.Property(x => x.EstimatedCost).HasColumnType("decimal(18,2)"));

        // Global Query Filter cho Soft Delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression ConvertFilterExpression(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var notDeleted = Expression.Not(property);
        return Expression.Lambda(notDeleted, parameter);
    }
}