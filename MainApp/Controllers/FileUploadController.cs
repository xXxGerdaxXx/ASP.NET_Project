using MainApp.Models;
using Microsoft.AspNetCore.Mvc;


public class FileUploadController(IWebHostEnvironment env) : Controller
{
    private readonly IWebHostEnvironment _env = env;

    [HttpPost]
    public async Task<IActionResult> Upload(FileUploadViewModel model)
    {
        if (!ModelState.IsValid || model.File == null || model.File.Length == 0)
            return BadRequest(new { success = false, message = "Invalid file upload" });


        if (string.IsNullOrEmpty(model.Folder))
            return BadRequest(new { success = false, message = "Folder name is required" });


        var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", model.Folder);
        Directory.CreateDirectory(uploadFolder);


        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.File.FileName)}";
        var filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/{model.Folder}/{fileName}";

        return Json(new { success = true, filePath = relativePath });
    }

}
