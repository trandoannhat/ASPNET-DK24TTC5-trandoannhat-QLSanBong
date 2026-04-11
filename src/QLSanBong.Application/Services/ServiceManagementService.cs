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
        return new ApiResponse<IEnumerable<ServiceDto>>(dtos, "Thành công");
    }

    public async Task<ApiResponse<ServiceDto>> GetServiceByIdAsync(Guid id)
    {
        var service = await unitOfWork.Services.GetByIdAsync(id);
        if (service == null) return new ApiResponse<ServiceDto>("Không tìm thấy mặt hàng.");

        var dto = mapper.Map<ServiceDto>(service);
        return new ApiResponse<ServiceDto>(dto, "Thành công");
    }

    public async Task<ApiResponse<Guid>> SaveServiceAsync(CreateUpdateServiceDto request)
    {
        if (request.Id == Guid.Empty)
        {
            // Thêm mới
            var newService = new Service
            {
                Name = request.Name,
                Price = request.Price,
                Unit = request.Unit,
                Category = request.Category
            };
            await unitOfWork.Services.AddAsync(newService);
            await unitOfWork.CompleteAsync();
            return new ApiResponse<Guid>(newService.Id, "Thêm mới thành công.");
        }
        else
        {
            // Cập nhật
            var existingService = await unitOfWork.Services.GetByIdAsync(request.Id);
            if (existingService == null) return new ApiResponse<Guid>("Không tìm thấy mặt hàng để sửa.");

            existingService.Name = request.Name;
            existingService.Price = request.Price;
            existingService.Unit = request.Unit;
            existingService.Category = request.Category;

            unitOfWork.Services.Update(existingService);
            await unitOfWork.CompleteAsync();
            return new ApiResponse<Guid>(existingService.Id, "Cập nhật thành công.");
        }
    }

    public async Task<ApiResponse<string>> DeleteServiceAsync(Guid id)
    {
        var service = await unitOfWork.Services.GetByIdAsync(id);
        if (service == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy mặt hàng." };

        unitOfWork.Services.Delete(service);
        await unitOfWork.CompleteAsync();
        return new ApiResponse<string>(id.ToString(), "Đã xóa thành công.");
    }
}