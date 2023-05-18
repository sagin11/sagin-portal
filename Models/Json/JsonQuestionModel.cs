namespace SaginPortal.Models.ExamModels;

public class JsonQuestionModel
{
    public int ExamId { get; set; }
    public string QuestionText { get; set; }
    public int Type { get; set; }
    public Answers[] Answers { get; set; }
}

public class Answers
{
    public int IsCorrect { get; set; }
    public string Content { get; set; }
}

