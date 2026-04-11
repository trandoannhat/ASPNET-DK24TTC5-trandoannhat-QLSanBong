using Microsoft.AspNetCore.Http;

namespace QLSanBong.Application.Interfaces;

public interface IVnPayService
{
    /// <summary>
    /// Tạo URL chuyển hướng đến cổng thanh toán VnPay
    /// </summary>
    string CreatePaymentUrl(HttpContext context, Guid bookingId, decimal amountToPay);

    /// <summary>
    /// Kiểm tra tính hợp lệ của chữ ký phản hồi từ VnPay
    /// </summary>
    bool ValidateSignature(IQueryCollection queryData);
}