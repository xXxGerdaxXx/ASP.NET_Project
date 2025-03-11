using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MainApp.Models;

public class FileUploadViewModel
{
    [Required]
    public IFormFile File { get; set; } = null!;
}
