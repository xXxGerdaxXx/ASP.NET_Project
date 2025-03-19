using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class FileService
{
    public async Task<string?> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            return null;

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folderName);
        Directory.CreateDirectory(uploadsPath); // ✅ Ensure the folder exists

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{folderName}/{fileName}"; // ✅ Returns relative URL for database storage
    }
}
