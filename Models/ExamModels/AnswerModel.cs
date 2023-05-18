using System.ComponentModel.DataAnnotations.Schema;

namespace SaginPortal.Models.ExamModels; 

[Table("Answers")]

public class AnswerModel
{
    public int Id { get; set; }
    public int? ExamId { get; set; }
    public int QuestionId { get; set; }
    public string Content { get; set; }
    public bool IsCorrect { get; set; }
}

