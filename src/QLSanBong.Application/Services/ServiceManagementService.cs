using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLSanBong.Application.DTOs.Service;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Wrappers;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Interfaces;

namespace QLSanBong.Application.Services;

public class ServiceManagementService(IUnitOfWork unitOfWork, IMapper mapper) : IServiceManagementService
{
    public async Task<ApiResponse<IEnumerable<ServiceDto>>> GetAllServicesAsync()
    {
        var services = await unitOfWork.Services.GetAllQueryable().ToListAsync();
        var dtos = mapper.Map<IEnumerable<ServiceDto>>(services);

        return ApiResponse<IEnumerable<ServiceDto>>.SuccessResponse(dtos, "Lấy danh sách dịch vụ thành công");
    }

    public async Task<ApiResponse<ServiceDto>> GetByIdAsync(Guid id)
    {
        var service = await unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
            return ApiResponse<ServiceDto>.FailureResponse("Dịch vụ không tồn tại.");

        var dto = mapper.Map<ServiceDto>(service);
        return ApiResponse<ServiceDto>.SuccessResponse(dto, "Thành công");
    }

    public async Task<ApiResponse<Guid>> SaveServiceAsync(CreateUpdateServiceDto request)
    {
        if (request.Id == Guid.Empty)
        {
            // Trường hợp thêm mới dịch vụ
            var newService = new Service
            {
                Name = request.Name,
                Price = request.Price,
                Unit = request.Unit,
                Category = request.Category
            };

            await unitOfWork.Services.AddAsync(newService);
            await unitOfWork.CompleteAsync();

            return ApiResponse<Guid>.SuccessResponse(newService.Id, "Thêm mới dịch vụ thành công.", "Create");
        }

        // Trường hợp cập nhật dịch vụ hiện có
        var existingService = await unitOfWork.Services.GetByIdAsync(request.Id);
        if (existingService == null)
            return ApiResponse<Guid>.FailureResponse("Không tìm thấy thông tin dịch vụ để cập nhật.");

        existingService.Name = request.Name;
        existingService.Price = request.Price;
        existingService.Unit = request.Unit;
        existingService.Category = request.Category;

        unitOfWork.Services.Update(existingService);
        await unitOfWork.CompleteAsync();

        return ApiResponse<Guid>.SuccessResponse(existingService.Id, "Cập nhật dịch vụ thành công.", "Update");
    }

    public async Task<ApiResponse<string>> DeleteServiceAsync(Guid id)
    {
        var service = await unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
            return ApiResponse<string>.FailureResponse("Dịch vụ không tồn tại.");

        unitOfWork.Services.Delete(service);
        await unitOfWork.CompleteAsync();

        return ApiResponse<string>.SuccessResponse(id.ToString(), "Xóa dịch vụ thành công.");
    }

    // Map interface cũ sang tên hàm mới để đảm bảo tính tương thích
    public Task<ApiResponse<ServiceDto>> GetServiceByIdAsync(Guid id) => GetByIdAsync(id);
}