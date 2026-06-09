namespace Delhivery.Domain.Entities;

public class FileMetadata : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    
    public Guid UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
}
