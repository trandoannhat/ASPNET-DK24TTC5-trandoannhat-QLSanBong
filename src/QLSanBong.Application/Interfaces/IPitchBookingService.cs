using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Common.Wrappers;

namespace QLSanBong.Application.Interfaces;

public interface IPitchBookingService
{
    // Nghiệp vụ Sân bóng
    Task<ApiResponse<PitchDto>> GetPitchByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CreatePitchAsync(CreatePitchDto request);
    Task<ApiResponse<string>> UpdatePitchAsync(UpdatePitchDto request);
    Task<ApiResponse<string>> DeletePitchAsync(Guid id);
    Task<ApiResponse<IEnumerable<PitchDto>>> GetAllPitchesAsync();

    // Nghiệp vụ Đặt sân
    Task<ApiResponse<Guid>> CreateBookingAsync(CreatePitchBookingDto request, Guid currentUserId);
    Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetMyBookingsAsync(Guid userId);
    Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetAllBookingsAsync(DateTime? fromDate, DateTime? toDate);
    Task<ApiResponse<string>> UpdateBookingStatusAsync(UpdatePitchBookingStatusDto request);
    Task<ApiResponse<Guid>> CreateAdminBookingAsync(CreateAdminPitchBookingDto request);
    Task<ApiResponse<string>> CancelMyBookingAsync(Guid bookingId, Guid currentUserId);
    Task<ApiResponse<IEnumerable<PitchDto>>> GetAvailablePitchesAsync(DateTime date, string startTime, string endTime, string? pitchType);
    Task<ApiResponse<string>> RescheduleMyBookingAsync(Guid bookingId, Guid currentUserId, DateTime newDate, string newStartTime, string newEndTime);
    // Dịch vụ đi kèm
    Task<ApiResponse<string>> AddServiceToBookingAsync(Guid bookingId, Guid serviceId, int quantity);
    Task<ApiResponse<PitchBookingDetailDto>> GetBookingDetailsAsync(Guid id);
    Task<ApiResponse<string>> ConfirmVnPayDepositAsync(Guid bookingId);
}