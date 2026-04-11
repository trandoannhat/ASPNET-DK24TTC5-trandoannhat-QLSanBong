using QLSanBong.Application.DTOs.Service;
using QLSanBong.Common.Wrappers;

namespace QLSanBong.Application.Interfaces;

public interface IServiceManagementService
{
    Task<ApiResponse<IEnumerable<ServiceDto>>> GetAllServicesAsync();
    Task<ApiResponse<ServiceDto>> GetServiceByIdAsync(Guid id);
    Task<ApiResponse<Guid>> SaveServiceAsync(CreateUpdateServiceDto request); // Dùng chung cho Create & Update
    Task<ApiResponse<string>> DeleteServiceAsync(Guid id);
}