namespace SaginPortal.Models.ExamModels; 

public class ResponseModel {
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int QuestionId { get; set; }
    public int StudentId { get; set; }
    public string? Response { get; set; }
}