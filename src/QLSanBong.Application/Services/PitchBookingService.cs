using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Wrappers;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Enums;
using QLSanBong.Domain.Interfaces;

namespace QLSanBong.Application.Services;

public class PitchBookingService(IUnitOfWork unitOfWork, IMapper mapper) : IPitchBookingService
{
    public async Task<ApiResponse<Guid>> CreateBookingAsync(CreatePitchBookingDto request, Guid currentUserId)
    {
        if (!TimeSpan.TryParse(request.StartTime, out TimeSpan start) || !TimeSpan.TryParse(request.EndTime, out TimeSpan end) || start >= end)
            return ApiResponse<Guid>.FailureResponse("Thời gian đặt sân không hợp lệ.");

        var pitch = await unitOfWork.Pitches.GetByIdAsync(request.PitchId);
        if (pitch == null) return ApiResponse<Guid>.FailureResponse("Sân bóng không tồn tại.");

        // Kiểm tra lịch trùng
        var isOverlapped = await unitOfWork.PitchBookings.GetAllQueryable()
            .AnyAsync(b => b.PitchId == request.PitchId && b.BookingDate.Date == request.BookingDate.Date
                        && b.Status != BookingStatus.Cancelled && start < b.EndTime && end > b.StartTime);

        if (isOverlapped) return ApiResponse<Guid>.FailureResponse("Khung giờ này đã có người đặt.");

        var newBooking = new PitchBooking
        {
            UserId = currentUserId,
            PitchId = request.PitchId,
            BookingDate = request.BookingDate.Date,
            StartTime = start,
            EndTime = end,
            TotalPrice = pitch.PricePerHour * (decimal)(end - start).TotalHours,
            Status = BookingStatus.Pending,
            Notes = request.Notes
        };

        await unitOfWork.PitchBookings.AddAsync(newBooking);
        await unitOfWork.CompleteAsync();

        return ApiResponse<Guid>.SuccessResponse(newBooking.Id, "Đặt sân thành công, vui lòng chờ duyệt.");
    }

    public async Task<ApiResponse<string>> ConfirmVnPayDepositAsync(Guid bookingId)
    {
        var booking = await unitOfWork.PitchBookings.GetByIdAsync(bookingId);
        if (booking == null) return ApiResponse<string>.FailureResponse("Không tìm thấy lịch đặt.");

        booking.Status = BookingStatus.Approved;
        booking.Notes = string.IsNullOrEmpty(booking.Notes) ? "[ĐÃ CỌC VNPAY]" : $"{booking.Notes} | [ĐÃ CỌC VNPAY]";

        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();
        return ApiResponse<string>.SuccessResponse(booking.Id.ToString(), "Xác nhận cọc thành công.");
    }

    public async Task<ApiResponse<PitchBookingDetailDto>> GetBookingDetailsAsync(Guid id)
    {
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.Pitch).Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return ApiResponse<PitchBookingDetailDto>.FailureResponse("Hóa đơn không tồn tại.");

        var services = await unitOfWork.BookingServices.GetAllQueryable()
            .Include(bs => bs.Service).Where(bs => bs.BookingId == id).ToListAsync();

        var dto = mapper.Map<PitchBookingDetailDto>(booking);
        dto.PurchasedServices = services.Select(s => new BookingServiceItemDto
        {
            ServiceName = s.Service.Name,
            Quantity = s.Quantity,
            UnitPrice = s.UnitPrice,
            TotalAmount = s.Quantity * s.UnitPrice
        }).ToList();

        return ApiResponse<PitchBookingDetailDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<IEnumerable<PitchDto>>> GetAllPitchesAsync()
    {
        var pitches = await unitOfWork.Pitches.GetAllQueryable().ToListAsync();
        return ApiResponse<IEnumerable<PitchDto>>.SuccessResponse(mapper.Map<IEnumerable<PitchDto>>(pitches));
    }

    public async Task<ApiResponse<PitchDto>> GetPitchByIdAsync(Guid id)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(id);
        return pitch == null
            ? ApiResponse<PitchDto>.FailureResponse("Không tìm thấy sân.")
            : ApiResponse<PitchDto>.SuccessResponse(mapper.Map<PitchDto>(pitch));
    }

    public async Task<ApiResponse<Guid>> CreatePitchAsync(CreatePitchDto request)
    {
        var pitch = new Pitch { Name = request.Name, PitchType = request.PitchType, PricePerHour = request.PricePerHour };
        await unitOfWork.Pitches.AddAsync(pitch);
        await unitOfWork.CompleteAsync();
        return ApiResponse<Guid>.SuccessResponse(pitch.Id, "Thêm sân thành công.");
    }

    public async Task<ApiResponse<string>> UpdatePitchAsync(UpdatePitchDto request)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(request.Id);
        if (pitch == null) return ApiResponse<string>.FailureResponse("Không tìm thấy sân.");

        pitch.Name = request.Name;
        pitch.PitchType = request.PitchType;
        pitch.PricePerHour = request.PricePerHour;

        unitOfWork.Pitches.Update(pitch);
        await unitOfWork.CompleteAsync();
        return ApiResponse<string>.SuccessResponse(pitch.Id.ToString(), "Cập nhật thành công.");
    }

    public async Task<ApiResponse<string>> DeletePitchAsync(Guid id)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(id);
        if (pitch == null) return ApiResponse<string>.FailureResponse("Không tìm thấy sân.");

        unitOfWork.Pitches.Delete(pitch);
        await unitOfWork.CompleteAsync();
        return ApiResponse<string>.SuccessResponse(id.ToString(), "Xóa thành công.");
    }

    public async Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetMyBookingsAsync(Guid userId)
    {
        var bookings = await unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.User).Include(b => b.Pitch).Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate).ToListAsync();
        return ApiResponse<IEnumerable<PitchBookingDto>>.SuccessResponse(mapper.Map<IEnumerable<PitchBookingDto>>(bookings));
    }

    public async Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetAllBookingsAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = unitOfWork.PitchBookings.GetAllQueryable().Include(b => b.User).Include(b => b.Pitch).AsQueryable();
        if (fromDate.HasValue) query = query.Where(b => b.BookingDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(b => b.BookingDate <= toDate.Value.Date);

        var bookings = await query.OrderByDescending(b => b.BookingDate).ToListAsync();
        return ApiResponse<IEnumerable<PitchBookingDto>>.SuccessResponse(mapper.Map<IEnumerable<PitchBookingDto>>(bookings));
    }

    public async Task<ApiResponse<string>> UpdateBookingStatusAsync(UpdatePitchBookingStatusDto request)
    {
        var booking = await unitOfWork.PitchBookings.GetByIdAsync(request.BookingId);
        if (booking == null) return ApiResponse<string>.FailureResponse("Không tìm thấy lịch.");

        booking.Status = request.Status;
        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();
        return ApiResponse<string>.SuccessResponse(booking.Id.ToString(), "Cập nhật trạng thái thành công.");
    }

    public async Task<ApiResponse<Guid>> CreateAdminBookingAsync(CreateAdminPitchBookingDto request)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(request.PitchId);
        if (pitch == null) return ApiResponse<Guid>.FailureResponse("Sân không tồn tại.");

        var user = await unitOfWork.Users.GetAllQueryable().FirstOrDefaultAsync(u => u.PhoneNumber == request.CustomerPhone)
                   ?? new User { FullName = request.CustomerName, PhoneNumber = request.CustomerPhone, Email = $"{request.CustomerPhone}@guest.local", PasswordHash = "GUEST", Role = UserRole.Customer };

        if (user.Id == Guid.Empty) await unitOfWork.Users.AddAsync(user);

        var booking = new PitchBooking
        {
            UserId = user.Id,
            PitchId = request.PitchId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = BookingStatus.Approved,
            TotalPrice = pitch.PricePerHour * (decimal)(request.EndTime - request.StartTime).TotalHours,
            Notes = request.Notes
        };

        await unitOfWork.PitchBookings.AddAsync(booking);
        await unitOfWork.CompleteAsync();
        return ApiResponse<Guid>.SuccessResponse(booking.Id, "Tạo lịch đặt thành công.");
    }

    public async Task<ApiResponse<string>> CancelMyBookingAsync(Guid bookingId, Guid currentUserId)
    {
        var booking = await unitOfWork.PitchBookings.GetByIdAsync(bookingId);
        if (booking == null || booking.UserId != currentUserId) return ApiResponse<string>.FailureResponse("Thao tác không hợp lệ.");
        if (booking.Status == BookingStatus.Completed) return ApiResponse<string>.FailureResponse("Không thể hủy lịch đã hoàn thành.");

        booking.Status = BookingStatus.Cancelled;
        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();
        return ApiResponse<string>.SuccessResponse(bookingId.ToString(), "Hủy lịch thành công.");
    }

    public async Task<ApiResponse<IEnumerable<PitchDto>>> GetAvailablePitchesAsync(DateTime date, string startTime, string endTime, string? pitchType)
    {
        if (!TimeSpan.TryParse(startTime, out TimeSpan start) || !TimeSpan.TryParse(endTime, out TimeSpan end) || start >= end)
            return ApiResponse<IEnumerable<PitchDto>>.FailureResponse("Giờ không hợp lệ.");

        var query = unitOfWork.Pitches.GetAllQueryable();
        if (!string.IsNullOrEmpty(pitchType)) query = query.Where(p => p.PitchType == pitchType);

        var allPitches = await query.ToListAsync();
        var busyIds = await unitOfWork.PitchBookings.GetAllQueryable()
            .Where(b => b.BookingDate.Date == date.Date && b.Status != BookingStatus.Cancelled && start < b.EndTime && end > b.StartTime)
            .Select(b => b.PitchId).ToListAsync();

        var dtos = mapper.Map<IEnumerable<PitchDto>>(allPitches.Where(p => !busyIds.Contains(p.Id)));
        return ApiResponse<IEnumerable<PitchDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<string>> AddServiceToBookingAsync(Guid bookingId, Guid serviceId, int quantity)
    {
        var booking = await unitOfWork.PitchBookings.GetByIdAsync(bookingId);
        var service = await unitOfWork.Services.GetByIdAsync(serviceId);
        if (booking == null || service == null) return ApiResponse<string>.FailureResponse("Dữ liệu không tồn tại.");

        await unitOfWork.BookingServices.AddAsync(new BookingService { BookingId = bookingId, ServiceId = serviceId, Quantity = quantity, UnitPrice = service.Price });
        booking.TotalPrice += service.Price * quantity;

        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();
        return ApiResponse<string>.SuccessResponse(bookingId.ToString(), "Đã thêm dịch vụ.");
    }

    public async Task<ApiResponse<string>> RescheduleMyBookingAsync(Guid bookingId, Guid currentUserId, DateTime newDate, string newStartTime, string newEndTime)
    {
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.Pitch)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return ApiResponse<string>.FailureResponse("Không tìm thấy lịch đặt.");
        if (booking.UserId != currentUserId) return ApiResponse<string>.FailureResponse("Không có quyền thao tác.");
        if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
            return ApiResponse<string>.FailureResponse("Chỉ có thể đổi lịch khi đang Chờ duyệt hoặc Đã xác nhận.");

        if (!TimeSpan.TryParse(newStartTime, out TimeSpan start) || !TimeSpan.TryParse(newEndTime, out TimeSpan end) || start >= end)
            return ApiResponse<string>.FailureResponse("Giờ không hợp lệ.");

        bool isOverlapped = await unitOfWork.PitchBookings.GetAllQueryable()
            .AnyAsync(b => b.Id != bookingId
                        && b.PitchId == booking.PitchId
                        && b.BookingDate.Date == newDate.Date
                        && b.Status != BookingStatus.Cancelled
                        && start < b.EndTime
                        && end > b.StartTime);

        if (isOverlapped)
            return ApiResponse<string>.FailureResponse("Sân đã có người đặt vào giờ này. Vui lòng chọn giờ khác.");

        booking.BookingDate = newDate.Date;
        booking.StartTime = start;
        booking.EndTime = end;
        booking.TotalPrice = booking.Pitch.PricePerHour * (decimal)(end - start).TotalHours;
        booking.Status = BookingStatus.Pending;

        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();

        return ApiResponse<string>.SuccessResponse(booking.Id.ToString(), "Đổi lịch thành công. Vui lòng chờ xác nhận.");
    }
}