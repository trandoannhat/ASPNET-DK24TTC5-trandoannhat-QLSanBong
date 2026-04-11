using Microsoft.EntityFrameworkCore;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Interfaces.Base;
using System.Linq.Expressions;

namespace QLSanBong.Infrastructure.Data;

public class QLSanBongDbContext(DbContextOptions<QLSanBongDbContext> options) : DbContext(options)
{
    // 1. Khai báo các bảng (DbSet)
    public DbSet<User> Users { get; set; }
   

    // ==========================================
    // THÊM : BẢNG QUẢN LÝ SÂN BÓNG MINI
    // ==========================================
    public DbSet<Pitch> Pitches { get; set; }
    public DbSet<PitchBooking> PitchBookings { get; set; }
    // ==========================================
    // Thêm 3 dòng này vào DbContext của bạn
    public DbSet<Service> Services { get; set; }
    public DbSet<BookingService> BookingServices { get; set; }
    public DbSet<PitchMaintenance> PitchMaintenances { get; set; }

    // --- CẤU HÌNH TỰ ĐỘNG NGÀY GIỜ & SOFT DELETE ---
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Xử lý Ngày tháng (IAuditable)
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // Chặn không cho sửa ngày tạo
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;
            }
        }

        // 2. Xử lý Soft Delete (ISoftDelete) - Chặn lệnh xóa cứng
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // "Bẻ lái": Đang từ trạng thái Xóa (Deleted) -> chuyển thành Sửa (Modified)
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    // 2. Cấu hình chi tiết (Fluent API)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);




        // --- Config User ---
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);

            // --- THÊM DÒNG NÀY: Lưu Role dạng chữ ---
            e.Property(x => x.Role).HasConversion<string>();
            // -----------------------------------------
        });

        
        // ==========================================
        // THÊM : CẤU HÌNH BẢNG QUẢN LÝ SÂN BÓNG MINI
        // ==========================================
        modelBuilder.Entity<Pitch>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.PitchType).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<PitchBooking>(e =>
        {
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.Status).HasConversion<string>();

            e.HasOne(b => b.User)
            .WithMany(u => u.PitchBookings)
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.Restrict);
             
            e.HasOne(b => b.Pitch)
             .WithMany(p => p.PitchBookings)
             .HasForeignKey(b => b.PitchId)
             .OnDelete(DeleteBehavior.Restrict);
        });
        // ==========================================

        // --- TỰ ĐỘNG GÁN QUERY FILTER (WHERE IsDeleted = false) ---
        // Quét tất cả các Entity, bảng nào có ISoftDelete thì tự động thêm Filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
    }

    // Hàm bổ trợ tạo Expression e => !e.IsDeleted
    private static LambdaExpression ConvertFilterExpression(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var notDeleted = Expression.Not(property);
        // SỬA: Đã thêm dòng return thiếu ở code cũ
        return Expression.Lambda(notDeleted, parameter);
    }
}