namespace QLSanBong.MVC.Models;

public class PitchViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PitchType { get; set; }
    public decimal PricePerHour { get; set; }
    public string? ImageUrl { get; set; }

    //  có thể thêm các trường khác nếu cần hiển thị thêm (vd: Hình ảnh, Trạng thái...)
}
