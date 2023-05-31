using Microsoft.EntityFrameworkCore;

namespace SaginPortal.Models.ExamModels;

public class AddQuestionModel {
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string? QuestionText { get; set; }
    public int Type { get; set; }
    public int? Points { get; set; }
    public List<AnswerModel> Answers { get; set; }
    public AddQuestionModel() {
        Answers = new List<AnswerModel>();
    }
    
}