using System.ComponentModel.DataAnnotations;

namespace QLSanBong.Domain.Enums;



public enum ServiceCategory
{
    [Display(Name = "Nước giải khát")]
    Beverage = 1,//// Nước giải khát (Nước suối, Bò húc, Revive...)

    [Display(Name = "Đồ ăn nhẹ")]
    Food = 2,//// Đồ ăn nhẹ (Mì tôm trứng, xúc xích...)

    [Display(Name = "Thiết bị (Bóng, Áo bib...)")]
    Equipment = 3,//// Thuê trang thiết bị (Quả bóng, áo bib, giày đá phủi...)

    [Display(Name = "Dịch vụ khác")]
    Other = 4// // Dịch vụ khác (Thuê trọng tài, thuê quay phát livestream...)
}