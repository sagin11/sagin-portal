using System.ComponentModel.DataAnnotations.Schema;

namespace SaginPortal.Models;

[Table("UploadedImages")]
public class UploadedImageModel {
    public int Id { get; set; }
    public string? OriginalFileName {get; set;}
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public DateTime UploadDate { get; set; }
    public int? OwnerId { get; set; }
}
