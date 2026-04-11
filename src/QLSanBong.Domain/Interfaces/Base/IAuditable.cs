namespace QLSanBong.Domain.Interfaces.Base;

// Tracking thời gian tạo và cập nhật bản ghi
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}