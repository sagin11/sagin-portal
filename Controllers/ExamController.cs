using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SaginPortal.Context;
using SaginPortal.Models;
using SaginPortal.Models.ExamModels;
using SaginPortal.Packages;

namespace SaginPortal.Controllers; 

public class ExamController : Controller {
    
    private readonly AppDbContext _dbContext;
    private readonly AppUrls _appUrls;

    public ExamController(AppDbContext dbContext, IOptions<AppUrls> appUrls) {
        _dbContext = dbContext;
        _appUrls = appUrls.Value;
    }

    [HttpGet]
    [Route("/Exam/{examUrl}")]
    public async Task<IActionResult> Index(string examUrl = "") {
        var examConfiguration = await _dbContext.ExamConfiguration.FirstOrDefaultAsync(e => e.ExamUrl == $"Exam/{examUrl}");
        if (examConfiguration == null) {
            return NotFound();
        }
        var exam = await _dbContext.Exams.FirstOrDefaultAsync(e => e.Id == examConfiguration.ExamId);

        ViewBag.Exam = exam!;
        ViewBag.ExamConfiguration = examConfiguration;
        
        HttpContext.Session.SetInt32("ExamId", exam.Id);
        
        return View();
    }

    [HttpPost]
    [Route("/Exam")]
    public async Task<IActionResult> Exam(StudentInfoModel model) {
        var student = model;
        var examId = HttpContext.Session.GetInt32("ExamId");
        var examConfiguration = await _dbContext.ExamConfiguration.FirstOrDefaultAsync(e => e.ExamId == examId);
        if (examId == null) {
            return NotFound();
        }

        var studentInfo = _dbContext.StudentsInfo.Add(new StudentInfoModel() {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Class = student.Class,
            NumberInLogbook = student.NumberInLogbook,
            ExamId = examId.Value
        });
        
        await _dbContext.SaveChangesAsync();

        Console.WriteLine(examConfiguration.RandomizeQuestions);
        
        if (examConfiguration.RandomizeQuestions) {
            var questions = await new ShuffleList(_dbContext).Shuffle(examId.Value);

            foreach (var question in questions) {
                _dbContext.Responses.Add(new ResponseModel() {
                    ExamId = examId.Value,
                    QuestionId = question.Id,
                    StudentId = studentInfo.Entity.Id
                });
            }
        } else {
            var questions = await _dbContext.Questions.Where(q => q.ExamId == examId).ToListAsync();
            foreach (var question in questions) {
                _dbContext.Responses.Add(new ResponseModel() {
                    ExamId = examId.Value,
                    QuestionId = question.Id,
                    StudentId = studentInfo.Entity.Id
                });
            }
        }
        
        await _dbContext.SaveChangesAsync();
        
        HttpContext.Session.SetInt32("StudnetId", studentInfo.Entity.Id);

        return RedirectToAction("ExamStarted", "Exam");
    }

    [HttpGet]
    [Route("/ExamStarted")]
    public async Task<IActionResult> ExamStarted() {
        // TODO: Trzeba przechowywać w sesji listę id z Responses, które są posortowane (pytania mogą być wylosowane).
        return View();
    }
}