
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Common.Wrappers;

namespace QLSanBong.Application.Interfaces;

public interface IPitchBookingService
{
    Task<ApiResponse<string>> ConfirmVnPayDepositAsync(Guid bookingId);
    // ==========================================
    // QUẢN LÝ DANH MỤC SÂN BÓNG (CRUD CHO ADMIN)
    // ==========================================
    Task<ApiResponse<PitchDto>> GetPitchByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreatePitchAsync(CreatePitchDto request);
    Task<ApiResponse<string>> UpdatePitchAsync(UpdatePitchDto request);
    Task<ApiResponse<string>> DeletePitchAsync(Guid id);

    // Lấy danh sách sân bóng
    Task<ApiResponse<IEnumerable<PitchDto>>> GetAllPitchesAsync();

    // Khách tự đặt sân
    Task<ApiResponse<Guid>> CreateBookingAsync(CreatePitchBookingDto request, Guid currentUserId);

    // Lịch sử đặt sân của khách
    Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetMyBookingsAsync(Guid userId);

    // Admin xem tất cả lịch đặt sân
    Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetAllBookingsAsync(DateTime? fromDate, DateTime? toDate);




    // Admin đổi trạng thái (Duyệt/Hủy/Hoàn thành)
    Task<ApiResponse<string>> UpdateBookingStatusAsync(UpdatePitchBookingStatusDto request);

    // Admin tạo lịch cho khách vãng lai
    Task<ApiResponse<Guid>> CreateAdminBookingAsync(CreateAdminPitchBookingDto request);

    // Khách hủy lịch
    Task<ApiResponse<string>> CancelMyBookingAsync(Guid bookingId, Guid currentUserId);

    // Khách đổi lịch
    Task<ApiResponse<string>> RescheduleMyBookingAsync(Guid bookingId, Guid currentUserId, DateTime newDate, string newStartTime, string newEndTime);

    // Tìm sân trống
    Task<ApiResponse<IEnumerable<PitchDto>>> GetAvailablePitchesAsync(DateTime date, string startTime, string endTime, string? pitchType);

    // ==========================================
    // QUẢN LÝ HÓA ĐƠN & DỊCH VỤ PHÁT SINH
    // ==========================================

    // Thêm nước uống/thiết bị vào hóa đơn
    Task<ApiResponse<string>> AddServiceToBookingAsync(Guid bookingId, Guid serviceId, int quantity);
    Task<ApiResponse<PitchBookingDetailDto>> GetBookingDetailsAsync(Guid id);


}