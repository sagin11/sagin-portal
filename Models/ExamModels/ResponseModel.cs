﻿namespace SaginPortal.Models.ExamModels; 

public class ResponseModel {
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
    public string? Response { get; set; }
}