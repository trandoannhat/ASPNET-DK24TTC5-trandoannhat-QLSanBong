using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Exceptions;

namespace QLSanBong.API.Controllers;


[Route("api/[controller]")]
public class FilesController(IFileService fileService) : BaseApiController
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string folder = "general")
    {
        if (file == null) return Error("Chưa chọn file");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return Error("Chỉ hỗ trợ định dạng ảnh (.jpg, .png, .webp)");

        if (file.Length > 5 * 1024 * 1024)
            return Error("Dung lượng file tối đa là 5MB");

        var url = await fileService.UploadImageAsync(file, folder);
        return Success(new { url }, "Upload ảnh thành công", "Upload File");
    }
}