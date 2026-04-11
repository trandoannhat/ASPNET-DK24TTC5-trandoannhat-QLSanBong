using System.ComponentModel.DataAnnotations;

namespace QLSanBong.Domain.Enums;

public enum ServiceCategory
{
    [Display(Name = "Nước giải khát")]
    Beverage = 1,   // Các loại nước uống, trà đá...

    [Display(Name = "Đồ ăn nhẹ")]
    Food = 2,       // Mì tôm, xúc xích, đồ ăn nhanh

    [Display(Name = "Cho thuê đồ")]
    Equipment = 3,  // Thuê giày, áo bib, quả bóng

    [Display(Name = "Dịch vụ khác")]
    Other = 4       // Thuê trọng tài, quay phim, tổ chức giải
}