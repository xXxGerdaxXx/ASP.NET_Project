using Microsoft.AspNetCore.Http;
using Infrastructure.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class FileService : IFileService
{
    public async Task<string?> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            return null;

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folderName);
        Directory.CreateDirectory(uploadsPath); 

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{folderName}/{fileName}"; 
    }
}
