using Microsoft.EntityFrameworkCore;

namespace SaginPortal.Models;


public class AnswerModel
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int QuestionId { get; set; }
    public string Content { get; set; }
    public bool IsCorrect { get; set; }
}

public class QuestionModel
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string QuestionText { get; set; }
}

public class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationTime { get; set; }
    public int CreatorId { get; set; }
}