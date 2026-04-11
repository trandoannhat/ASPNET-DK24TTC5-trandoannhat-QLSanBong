using QLSanBong.Domain.Entities.Base; 
using QLSanBong.Domain.Enums;
using QLSanBong.Domain.Interfaces.Base;

namespace QLSanBong.Domain.Entities;

public class User : AuditableEntity, ISoftDelete
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.Client;

    public ICollection<PitchBooking> PitchBookings { get; set; } = new List<PitchBooking>();

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Logic khi muốn khôi phục lại tài khoản đã xóa
    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}