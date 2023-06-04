using System.ComponentModel.DataAnnotations.Schema;

namespace SaginPortal.Models.ExamModels; 

[Table("Questions")]
public class QuestionModel
{
    public int Id { get; set; }
    public int? ExamId { get; set; }
    public string? QuestionText { get; set; }
    public int Points { get; set; }
    // Closed = 0,
    // Multiple = 1,   
    // TextWithCheck = 2,
    // TextWithOutCheck = 3
    public int Type { get; set; }
}