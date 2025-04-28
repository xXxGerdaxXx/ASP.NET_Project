namespace Infrastructure.Entities;


public class FileEntity
{
    public int FileId { get; set; }  
    public string FileName { get; set; } = null!;  
    public string FilePath { get; set; } = null!; 
    public string FileType { get; set; } = null!;  
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public string UserId { get; set; } = null!;  
    public UserEntity? User { get; set; } 

    public int? ProjectId { get; set; }  
    public ProjectEntity? Project { get; set; }  
}
