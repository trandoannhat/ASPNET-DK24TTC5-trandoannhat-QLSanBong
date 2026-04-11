using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QLSanBong.Application.Interfaces;

namespace QLSanBong.Infrastructure.Services;

public class LocalFileService(IWebHostEnvironment webHostEnvironment) : IFileService
{
    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File không hợp lệ");

        var rootPath = webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsFolder = Path.Combine(rootPath, "uploads", folder);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{fileName}-{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{folder}/{uniqueFileName}";
    }
}