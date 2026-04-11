using Microsoft.AspNetCore.Mvc;
using QLSanBong.Common.Wrappers;

namespace QLSanBong.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // 1. Trả về 200 OK (Thành công)
    protected IActionResult Success<T>(T data, string message = "Thành công", string action = "Operation")
        => Ok(ApiResponse<T>.SuccessResponse(data, message, action));

    // 2. Trả về 400 Bad Request (Lỗi)
    protected IActionResult Error(string message, string action = "Error")
        => BadRequest(ApiResponse<string>.FailureResponse(message, action));

    // 3. Trả về 200 OK (Có phân trang)
    protected IActionResult Paged<T>(T data, int page, int size, int total, string action = "Get List")
        => Ok(new PagedResponse<T>(data, page, size, total, action));

    // 4. Trả về 201 Created (Vừa tạo mới xong)
    protected IActionResult CreatedResource<T>(T data, string message = "Tạo mới thành công", string action = "Create")
        => StatusCode(201, ApiResponse<T>.SuccessResponse(data, message, action));
}