using System.ComponentModel.DataAnnotations.Schema;

namespace SaginPortal.Models.ExamModels; 

[Table("ExamConfigurations")]
public class ExamConfigurationModel {
    public int Id { get; set; }
    
    [ForeignKey("Exam")]
    public int ExamId { get; set; }

    public int? QuestionCount { get; set; }
    public bool RandomizeQuestions { get; set; }
    public TimeSpan QuestionTime { get; set; }
    public string? ExamUrl { get; set; }
    public bool FirstName { get; set; }
    public bool LastName { get; set; }
    public bool Class { get; set; }
    public bool NumberInLogbook { get; set; }
    
}