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
    // Closed = 0,
    // Multiple = 1,   
    // TextWithCheck = 2,
    // TextWithOutCheck = 3
    public int Type { get; set; }
}

public class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationTime { get; set; }
    public int CreatorId { get; set; }
    public string Category { get; set; }
    public string Status { get; set; }
}
