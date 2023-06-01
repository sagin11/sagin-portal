using System.ComponentModel.DataAnnotations;

namespace SaginPortal.Models.ExamModels; 

public class StudentInfoModel {
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Class { get; set; }
    public string? NumberInLogbook { get; set; }
    public int ExamId { get; set; }
}