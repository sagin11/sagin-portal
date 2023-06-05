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
        HttpContext.Session.SetString("ExamName", exam.Name);
        
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

        var results = new ResultsModel() {
            ExamId = examId.Value,
            StudentId = studentInfo.Entity.Id,
            MaxPoints = 0
        };
        
        List<QuestionModel> questions;
        
        if (examConfiguration.RandomizeQuestions) 
            questions = await new ShuffleList(_dbContext).Shuffle(examId.Value);
        else questions = await _dbContext.Questions.Where(q => q.ExamId == examId).ToListAsync();
        
        foreach (var question in questions) {
            results.MaxPoints += question.Points;
            var response = await _dbContext.Responses.AddAsync(new ResponseModel() {
                ExamId = examId.Value,
                QuestionId = question.Id,
                StudentId = studentInfo.Entity.Id
            });
        }
        
        _dbContext.Results.Add(results);
        await _dbContext.SaveChangesAsync();

        var first = await _dbContext.Responses.FirstAsync(e => e.StudentId == studentInfo.Entity.Id);
        var last = await _dbContext.Responses
            .Where(e => e.StudentId == studentInfo.Entity.Id)
            .OrderByDescending(e => e.Id)
            .FirstAsync();

        HttpContext.Session.SetInt32("StudnetId", studentInfo.Entity.Id);
        HttpContext.Session.SetInt32("FirstResponseId", first.Id);
        HttpContext.Session.SetInt32("CurrentResponseId", first.Id);
        HttpContext.Session.SetInt32("LastResponseId", last.Id);
        return RedirectToAction("ExamStarted", "Exam");
    }

    [HttpGet]
    [Route("/ExamStarted")]
    public async Task<IActionResult> ExamStarted() {
        var examId = HttpContext.Session.GetInt32("ExamId");
        var studentId = HttpContext.Session.GetInt32("StudnetId");
        var firstResponseId = HttpContext.Session.GetInt32("FirstResponseId");
        var currentResponseId = HttpContext.Session.GetInt32("CurrentResponseId");
        var lastResponseId = HttpContext.Session.GetInt32("LastResponseId");
        
        
        var examName = HttpContext.Session.GetString("ExamName");
        
        if (examName == null || studentId == null) {
            HttpContext.Session.Clear();
            return BadRequest();
        }
        
        var response = await _dbContext.Responses.FirstAsync(r => r.Id == currentResponseId);
        var question = await _dbContext.Questions.FirstAsync(q => q.Id == response.QuestionId);
        var answers = await _dbContext.Answers.Where(a => a.QuestionId == question.Id).ToListAsync();
        
        ViewBag.examName = examName;
        ViewBag.question = question;
        ViewBag.answers = answers;

        return View();
    }

    [HttpPost]
    [Route("/ExamStarted")]
    public async Task<IActionResult> ExamStarted(SubmitQuestionModel model) {
        if (HttpContext.Session.GetInt32("ExamFinished") == 1) {
            return RedirectToAction("ExamFinished", "Exam");
        }
        
        var examId = HttpContext.Session.GetInt32("ExamId");
        var studentId = HttpContext.Session.GetInt32("StudnetId");
        var firstResponseId = HttpContext.Session.GetInt32("FirstResponseId");
        var currentResponseId = HttpContext.Session.GetInt32("CurrentResponseId");
        var lastResponseId = HttpContext.Session.GetInt32("LastResponseId");

        var examName = HttpContext.Session.GetString("ExamName");
        
        if (examName == null || studentId == null) {
            HttpContext.Session.Clear();
            return BadRequest();
        }
        
        var response = await _dbContext.Responses.FirstAsync(r => r.Id == currentResponseId);
        response.Response = model.answerContent;
        
        var result = await _dbContext.Results.FirstAsync(r => r.StudentId == studentId);

        var answers = await _dbContext.Answers.Where(a => a.ExamId == result.ExamId).ToListAsync();

        var forQuestion = await _dbContext.Questions.FirstAsync(q => q.ExamId == response.ExamId);  
        foreach (var answer in answers) {
            if (answer.IsCorrect && response.Response == answer.Content) {
                result.Points += forQuestion.Points;
            }
        }
        
        await _dbContext.SaveChangesAsync();

        currentResponseId = currentResponseId!.Value + 1;
        if (currentResponseId > lastResponseId) {
            HttpContext.Session.SetInt32("ExamFinished", 1);
            return RedirectToAction("ExamFinished", "Exam");
        }
        HttpContext.Session.SetInt32("CurrentResponseId", currentResponseId.Value);
        
        
        var question = await _dbContext.Questions.FirstAsync(q => q.Id == response.QuestionId);
        answers = await _dbContext.Answers.Where(a => a.QuestionId == question.Id).ToListAsync();
        
        ViewBag.examName = examName;
        ViewBag.question = question;
        ViewBag.answers = answers;

        
        return View();
    }

    [HttpGet]
    [Route("/ExamFinished")]
    public async Task<IActionResult> ExamFinished() {
        if (HttpContext.Session.GetInt32("ExamFinished") == 1) {
            var studentId = HttpContext.Session.GetInt32("StudnetId");
            var result = await _dbContext.Results.FirstAsync(r => r.StudentId == studentId);
            HttpContext.Session.Clear();

            ViewBag.result = result;
            return View();    
        } else {
            return StatusCode(403);
        }
    }

    [HttpPost]
    [Route("/generate_204")]
    public async Task<IActionResult> ExamCheat() {
        // var studentId = HttpContext.Session.GetInt32("StudentId");
        var currentResponseId = HttpContext.Session.GetInt32("CurrentResponseId");

        if (currentResponseId == null)
            return StatusCode(204);
        
        var response = await _dbContext.Responses.FirstAsync(r => r.Id == currentResponseId);

        response.Blurs++;
        
        _dbContext.Responses.Update(response);
        await _dbContext.SaveChangesAsync();

        if (response.Blurs == 3) {
            HttpContext.Session.SetInt32("ExamFinished", 1);
            HttpContext.Session.Remove("FirstResponseId");
            HttpContext.Session.Remove("CurrentResponseId");
            HttpContext.Session.Remove("LastResponseId");
            return Json(new { message = "Przykro mi" });
        }
        return StatusCode(204);
    }
}