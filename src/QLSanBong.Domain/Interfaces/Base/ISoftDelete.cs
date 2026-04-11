namespace QLSanBong.Domain.Interfaces.Base;

// Đánh dấu bản ghi đã bị xóa (Logical Delete) thay vì xóa vật lý khỏi Database
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }

    void Undo(); // Khôi phục dữ liệu
}