using System.ComponentModel.DataAnnotations.Schema;

namespace SaginPortal.Models.ExamModels; 

[Table("Exams")]
public class ExamModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreationTime { get; set; }
    public int CreatorId { get; set; }
    public int Category { get; set; }
    public string? Status { get; set; }
}
