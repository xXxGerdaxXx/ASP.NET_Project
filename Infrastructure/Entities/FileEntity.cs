namespace Infrastructure.Entities;


public class FileEntity
{
    public int FileId { get; set; }  // Primary Key
    public string FileName { get; set; } = null!;  // Original file name
    public string FilePath { get; set; } = null!;  // Path where file is stored
    public string FileType { get; set; } = null!;  // e.g., "image/png", "image/jpeg"
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Optional Foreign Keys
    public string UserId { get; set; } = null!;  
    public UserEntity? User { get; set; }  // Nullable navigation property

    public int? ProjectId { get; set; }  // Nullable foreign key
    public ProjectEntity? Project { get; set; }  // Nullable navigation property
}
