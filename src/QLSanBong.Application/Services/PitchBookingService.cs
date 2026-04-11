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
    public async Task<ApiResponse<string>> ConfirmVnPayDepositAsync(Guid bookingId)
    {
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return new ApiResponse<string> { Success = false, Message = "Không tìm thấy lịch đặt." };

        booking.Status = BookingStatus.Approved; // 1 = Tự động chuyển sang Đã duyệt

        // Tự động dán nhãn VNPay vào Ghi chú cho Admin dễ nhìn
        string vnpayTag = "[ ĐÃ CỌC VNPAY 30%]";

        if (string.IsNullOrEmpty(booking.Notes))
        {
            booking.Notes = vnpayTag;
        }
        else if (!booking.Notes.Contains("VNPAY")) // Tránh dán trùng nếu lỡ gọi 2 lần
        {
            booking.Notes = booking.Notes + " | " + vnpayTag;
        }

        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: booking.Id.ToString(), message: "Đã cập nhật cọc VNPay thành công.");
    }
    // ==========================================
    // QUẢN LÝ DANH MỤC SÂN BÓNG (CRUD CHO ADMIN)
    // ==========================================

    public async Task<ApiResponse<PitchDto>> GetPitchByIdAsync(Guid id)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(id);
        if (pitch == null) return new ApiResponse<PitchDto>("Không tìm thấy sân bóng.");
        
        var dto = mapper.Map<PitchDto>(pitch);
        return new ApiResponse<PitchDto>(dto, "Thành công");
    }

    public async Task<ApiResponse<Guid>> CreatePitchAsync(CreatePitchDto request)
    {
        var newPitch = new Pitch
        {
            Name = request.Name,
            PitchType = request.PitchType,
            PricePerHour = request.PricePerHour
        };

        await unitOfWork.Pitches.AddAsync(newPitch);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<Guid>(newPitch.Id, "Thêm sân bóng thành công.");
    }

    public async Task<ApiResponse<string>> UpdatePitchAsync(UpdatePitchDto request)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(request.Id);
        if (pitch == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy sân bóng." };

        pitch.Name = request.Name;
        pitch.PitchType = request.PitchType;
        pitch.PricePerHour = request.PricePerHour;

        unitOfWork.Pitches.Update(pitch);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: pitch.Id.ToString(), message: "Cập nhật sân bóng thành công.");
    }

    public async Task<ApiResponse<string>> DeletePitchAsync(Guid id)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(id);
        if (pitch == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy sân bóng." };

        // Xóa sân bóng (ISoftDelete sẽ tự động đánh dấu IsDeleted = true thay vì xóa cứng)
        unitOfWork.Pitches.Delete(pitch);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: id.ToString(), message: "Đã xóa sân bóng thành công.");
    }

    public async Task<ApiResponse<IEnumerable<PitchDto>>> GetAllPitchesAsync()
    {
        var pitches = await unitOfWork.Pitches.GetAllQueryable().ToListAsync();
        var dtos = mapper.Map<IEnumerable<PitchDto>>(pitches);
        return new ApiResponse<IEnumerable<PitchDto>>(dtos, "Thành công");
    }

    public async Task<ApiResponse<Guid>> CreateBookingAsync(CreatePitchBookingDto request, Guid currentUserId)
    {
        if (!TimeSpan.TryParse(request.StartTime, out TimeSpan startTime) || 
            !TimeSpan.TryParse(request.EndTime, out TimeSpan endTime))
        {
            return new ApiResponse<Guid> { Success = false, Message = "Định dạng giờ không hợp lệ (VD: 14:30)." };
        }

        if (startTime >= endTime)
        {
            return new ApiResponse<Guid> { Success = false, Message = "Giờ kết thúc phải lớn hơn giờ bắt đầu." };
        }

        var pitch = await unitOfWork.Pitches.GetAllQueryable().FirstOrDefaultAsync(p => p.Id == request.PitchId);
        if (pitch == null) return new ApiResponse<Guid> { Success = false, Message = "Sân bóng không tồn tại." };

        // Kiểm tra trùng giờ
        var targetDate = request.BookingDate.Date;
        bool isOverlapped = await unitOfWork.PitchBookings.GetAllQueryable()
            .AnyAsync(b => b.PitchId == request.PitchId
                        && b.BookingDate.Date == targetDate
                        && b.Status != BookingStatus.Cancelled
                        && startTime < b.EndTime
                        && endTime > b.StartTime);

        if (isOverlapped)
        {
            return new ApiResponse<Guid> { Success = false, Message = "Sân bóng này đã có người đặt trong khung giờ bạn chọn." };
        }

        var durationHours = (decimal)(endTime - startTime).TotalHours;
        var totalPrice = pitch.PricePerHour * durationHours;

        var newBooking = new PitchBooking
        {
            UserId = currentUserId,
            PitchId = request.PitchId,
            BookingDate = request.BookingDate.Date,
            StartTime = startTime,
            EndTime = endTime,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            Notes = request.Notes
        };

        await unitOfWork.PitchBookings.AddAsync(newBooking);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<Guid>(data: newBooking.Id, message: "Đặt sân thành công! Vui lòng chờ xác nhận.");
    }

    public async Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetMyBookingsAsync(Guid userId)
    {
        var bookings = await unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.User)
            .Include(b => b.Pitch)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime)
            .ToListAsync();

        var dtos = mapper.Map<IEnumerable<PitchBookingDto>>(bookings);
        return new ApiResponse<IEnumerable<PitchBookingDto>>(dtos, "Thành công");
    }

    public async Task<ApiResponse<IEnumerable<PitchBookingDto>>> GetAllBookingsAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.User)
            .Include(b => b.Pitch)
            .AsQueryable();

        if (fromDate.HasValue) query = query.Where(b => b.BookingDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(b => b.BookingDate <= toDate.Value.Date);

        var bookings = await query
            .OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime)
            .ToListAsync();

        var dtos = mapper.Map<IEnumerable<PitchBookingDto>>(bookings);
        return new ApiResponse<IEnumerable<PitchBookingDto>>(dtos, "Thành công");
    }

    public async Task<ApiResponse<string>> UpdateBookingStatusAsync(UpdatePitchBookingStatusDto request)
    {
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .FirstOrDefaultAsync(b => b.Id == request.BookingId);

        if (booking == null)
            return new ApiResponse<string> { Success = false, Message = "Không tìm thấy lịch đặt." };

        booking.Status = request.Status;
        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: booking.Id.ToString(), message: "Đã cập nhật trạng thái lịch đặt thành công.");
    }

    public async Task<ApiResponse<Guid>> CreateAdminBookingAsync(CreateAdminPitchBookingDto request)
    {
        var pitch = await unitOfWork.Pitches.GetByIdAsync(request.PitchId);
        if (pitch == null) return new ApiResponse<Guid>("Không tìm thấy sân bóng.");

        Guid userIdToSave;
        var existingUser = await unitOfWork.Users.GetAllQueryable()
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.CustomerPhone);

        if (existingUser != null)
        {
            userIdToSave = existingUser.Id;
        }
        else
        {
            var guestUser = new User
            {
                FullName = request.CustomerName,
                PhoneNumber = request.CustomerPhone,
                Email = $"{request.CustomerPhone}@guest.pitch.local",
                PasswordHash = "GUEST_NO_PASSWORD",
                Role = QLSanBong.Domain.Enums.UserRole.Client
            };

            await unitOfWork.Users.AddAsync(guestUser);
            await unitOfWork.CompleteAsync();
            userIdToSave = guestUser.Id;
        }

        var durationHours = (decimal)(request.EndTime - request.StartTime).TotalHours;

        var booking = new PitchBooking
        {
            UserId = userIdToSave,
            PitchId = request.PitchId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = pitch.PricePerHour * durationHours,
            Notes = request.Notes,
            Status = (BookingStatus)request.Status
        };

        await unitOfWork.PitchBookings.AddAsync(booking);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<Guid>(booking.Id, "Quản lý đã tạo lịch đặt sân thành công.");
    }

    public async Task<ApiResponse<string>> CancelMyBookingAsync(Guid bookingId, Guid currentUserId)
    {
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return new ApiResponse<string> { Success = false, Message = "Không tìm thấy lịch đặt." };

        if (booking.UserId != currentUserId)
            return new ApiResponse<string> { Success = false, Message = "Bạn không có quyền thao tác." };

        if (booking.Status == BookingStatus.Completed)
            return new ApiResponse<string> { Success = false, Message = "Không thể hủy lịch đã hoàn thành." };

        if (booking.Status == BookingStatus.Cancelled)
            return new ApiResponse<string> { Success = false, Message = "Lịch đã được hủy từ trước." };

        booking.Status = BookingStatus.Cancelled;
        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: booking.Id.ToString(), message: "Đã hủy lịch đặt sân thành công.");
    }

    public async Task<ApiResponse<string>> RescheduleMyBookingAsync(Guid bookingId, Guid currentUserId, DateTime newDate, string newStartTime, string newEndTime)
    {
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.Pitch)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy lịch đặt." };
        if (booking.UserId != currentUserId) return new ApiResponse<string> { Success = false, Message = "Không có quyền thao tác." };
        if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled)
            return new ApiResponse<string> { Success = false, Message = "Chỉ có thể đổi lịch khi đang Chờ duyệt hoặc Đã xác nhận." };

        if (!TimeSpan.TryParse(newStartTime, out TimeSpan start) || !TimeSpan.TryParse(newEndTime, out TimeSpan end) || start >= end)
            return new ApiResponse<string> { Success = false, Message = "Giờ không hợp lệ." };

        bool isOverlapped = await unitOfWork.PitchBookings.GetAllQueryable()
            .AnyAsync(b => b.Id != bookingId
                        && b.PitchId == booking.PitchId
                        && b.BookingDate.Date == newDate.Date
                        && b.Status != BookingStatus.Cancelled
                        && start < b.EndTime
                        && end > b.StartTime);

        if (isOverlapped)
            return new ApiResponse<string> { Success = false, Message = "Sân đã có người đặt vào giờ này. Vui lòng chọn giờ khác." };

        var durationHours = (decimal)(end - start).TotalHours;

        booking.BookingDate = newDate.Date;
        booking.StartTime = start;
        booking.EndTime = end;
        booking.TotalPrice = booking.Pitch.PricePerHour * durationHours;
        booking.Status = BookingStatus.Pending;

        unitOfWork.PitchBookings.Update(booking);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: booking.Id.ToString(), message: "Đổi lịch thành công. Vui lòng chờ xác nhận.");
    }

    public async Task<ApiResponse<IEnumerable<PitchDto>>> GetAvailablePitchesAsync(DateTime date, string startTime, string endTime, string? pitchType)
    {
        if (!TimeSpan.TryParse(startTime, out TimeSpan start) || !TimeSpan.TryParse(endTime, out TimeSpan end) || start >= end)
            return new ApiResponse<IEnumerable<PitchDto>> { Success = false, Message = "Giờ không hợp lệ." };

        var allPitchesQuery = unitOfWork.Pitches.GetAllQueryable();
        if (!string.IsNullOrEmpty(pitchType))
        {
            allPitchesQuery = allPitchesQuery.Where(p => p.PitchType == pitchType);
        }
        var allPitches = await allPitchesQuery.ToListAsync();

        var busyPitchIds = await unitOfWork.PitchBookings.GetAllQueryable()
            .Where(b => b.BookingDate.Date == date.Date
                     && b.Status != BookingStatus.Cancelled
                     && start < b.EndTime
                     && end > b.StartTime)
            .Select(b => b.PitchId)
            .ToListAsync();

        var availablePitches = allPitches.Where(p => !busyPitchIds.Contains(p.Id)).ToList();
        var dtos = mapper.Map<IEnumerable<PitchDto>>(availablePitches);

        return new ApiResponse<IEnumerable<PitchDto>>(dtos, "Thành công");
    }
   
    public async Task<ApiResponse<string>> AddServiceToBookingAsync(Guid bookingId, Guid serviceId, int quantity)
    {
        if (quantity <= 0) return new ApiResponse<string> { Success = false, Message = "Số lượng phải lớn hơn 0." };

        // 1. Kiểm tra Lịch đặt có tồn tại không
        var booking = await unitOfWork.PitchBookings.GetByIdAsync(bookingId);
        if (booking == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy hóa đơn đặt sân." };

        // 2. Kiểm tra Dịch vụ có tồn tại không
        var service = await unitOfWork.Services.GetByIdAsync(serviceId);
        if (service == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy dịch vụ/mặt hàng." };

        // 3. Tạo record ghi nhận chi tiết bán hàng
        var bookingService = new BookingService
        {
            BookingId = bookingId,
            ServiceId = serviceId,
            Quantity = quantity,
            UnitPrice = service.Price // Lưu cứng giá tại thời điểm bán
        };

        // 4. Cộng dồn tiền vào Tổng hóa đơn của Lịch đặt sân
        var additionalCost = service.Price * quantity;
        booking.TotalPrice += additionalCost;

        // 5. Lưu vào Database (Sử dụng Transaction ẩn của UnitOfWork)
        await unitOfWork.BookingServices.AddAsync(bookingService);
        unitOfWork.PitchBookings.Update(booking);

        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: booking.Id.ToString(), message: $"Đã thêm {quantity} {service.Unit} {service.Name} vào hóa đơn.");
    }
    public async Task<ApiResponse<PitchBookingDetailDto>> GetBookingDetailsAsync(Guid id)
    {
        // 1. Lấy thông tin đặt sân
        var booking = await unitOfWork.PitchBookings.GetAllQueryable()
            .Include(b => b.Pitch)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return new ApiResponse<PitchBookingDetailDto>("Không tìm thấy hóa đơn.");

        // 2. Lấy danh sách Nước/Bóng khách đã gọi thêm
        var services = await unitOfWork.BookingServices.GetAllQueryable()
            .Include(bs => bs.Service)
            .Where(bs => bs.BookingId == id)
            .ToListAsync();

        // 3. Đổ dữ liệu vào DTO
        var dto = new PitchBookingDetailDto
        {
            Id = booking.Id,
            CustomerName = booking.User.FullName,
            CustomerPhone = booking.User.PhoneNumber,
            PitchName = booking.Pitch.Name,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status.ToString(),
            Notes = booking.Notes,

            // Map danh sách dịch vụ
            PurchasedServices = services.Select(s => new BookingServiceItemDto
            {
                ServiceName = s.Service.Name,
                Quantity = s.Quantity,
                UnitPrice = s.UnitPrice,
                TotalAmount = s.Quantity * s.UnitPrice
            }).ToList()
        };

        return new ApiResponse<PitchBookingDetailDto>(dto, "Thành công");
    }
}