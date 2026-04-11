using System.ComponentModel.DataAnnotations;

namespace QLSanBong.MVC.Models
{
    public class CreatePitchViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sân")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại sân")]
        public string PitchType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá thuê")]
        [Range(10000, 10000000, ErrorMessage = "Giá thuê phải từ 10.000 đến 10.000.000 VNĐ")]
        public decimal PricePerHour { get; set; }

        // THÊM DÒNG NÀY ĐỂ HỨNG FILE ẢNH TỪ FORM HTML LÊN
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
    }
}
