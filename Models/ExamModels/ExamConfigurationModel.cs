using System.ComponentModel.DataAnnotations.Schema;
using Org.BouncyCastle.Asn1.X509;

namespace SaginPortal.Models.ExamModels; 

[Table("ExamConfigurations")]
public class ExamConfigurationModel {
    public int Id { get; set; }
    
    [ForeignKey("Exam")]
    public int ExamId { get; set; }

    public int? QuestionCount { get; set; }
    public bool RandomizeQuestions { get; set; }
    public TimeSpan QuestionTime { get; set; }
}