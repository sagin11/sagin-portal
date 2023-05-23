using System.ComponentModel.DataAnnotations.Schema;

namespace SaginPortal.Models.ExamModels; 

[Table("ExamConfigurations")]
public class ExamConfigurationModel {
    public int Id { get; set; }
    
    [ForeignKey("Exam")]
    public int ExamId { get; set; }

    public int? QuestionCount { get; set; }
    public bool RandomizeQuestions { get; set; }
}