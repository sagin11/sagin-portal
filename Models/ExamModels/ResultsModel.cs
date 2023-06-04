namespace SaginPortal.Models.ExamModels; 

public class ResultsModel {
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int Points { get; set; }
    public int MaxPoints { get; set; }
}