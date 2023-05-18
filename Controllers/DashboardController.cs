using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;
using SaginPortal.Packages;

namespace SaginPortal.Controllers;

[CheckLoginStatus]
public class DashboardController : Controller {
    private readonly AppDbContext _dbContext;

    public DashboardController(AppDbContext appDbContext) {
        _dbContext = appDbContext;
    }

    public async Task<IActionResult> Index() {

        var exams = await _dbContext.Exams.Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId"))
            .ToListAsync();

        // if (!(exams.Count > 0)) return View();

        var questions = await _dbContext.Questions.ToListAsync();
        var answers = await _dbContext.Answers.ToListAsync();
        var categories = await _dbContext.ExamCategories.ToListAsync();
        // ViewBag.categories = categories;
        Console.Write("PA PA");
        Console.Write(categories);
        Console.Write("PA PA");
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;

        return View();
    }

    [ExamIdValidator]
    [Route("Dashboard/Exam/{id:int}")]
    public async Task<IActionResult> Exam(int id = -1) {

        var test = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        
        if (test.Count <= 0) {
            return RedirectToAction("Login", "Account");
        }

        Console.WriteLine(test.Count);

        var exams = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        var questions = await _dbContext.Questions.Where(t => t.ExamId == id).ToListAsync();
        var answers = await _dbContext.Answers.Where(t => t.ExamId == id).ToListAsync();
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;
        return View();
    }

    [HttpPost]
    [Route("Dashboard/CreateExam")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExam(AddExamModel model) {
        var examName = model.Name;
        var examCategoryId = model.CategoryId;
        var examCategoryName = model.CategoryName;

        if (examName == null || (examCategoryId == null || examCategoryName == null)) {
            return RedirectToAction("Index", "Dashboard");
        }

        if (examCategoryId != null) {
            if (examCategoryName.Length > 0) {
                var category = new ExamCategoryModel() {
                    CategoryName = examCategoryName,
                    UserId = HttpContext.Session.GetInt32("UserId")!.Value
                };

                _dbContext.ExamCategories.Add(category);
                await _dbContext.SaveChangesAsync();

            }
        }   

        // error
        if (examName.Length > 200 || examCategoryName.Length > 30) {
            return RedirectToAction("Index", "Dashboard");
        }

        var exam = new ExamModel {
            Name = examName,
            CategoryId = examCategoryId,
            CreationTime = DateTime.Now,
            CreatorId = HttpContext.Session.GetInt32("UserId")!.Value,
            Status = "Disabled"
        };
        
        _dbContext.Exams.Add(exam);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> GetUserCategories() {
    
        var categories = await _dbContext.ExamCategories.Where(c => c.UserId == HttpContext.Session.GetInt32("UserId")!.Value).Select(c => new {
            c.Id, 
            c.CategoryName
        }).ToListAsync();
        
        if (categories.Count <= 0) return Json(new { data = "No data." });
        
        return Json(new { data = categories});
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory() {
        
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var json = Newtonsoft.Json.JsonConvert.DeserializeObject<AddExamCategoryModel>(requestBody);
        
        var categoryName = json?.CategoryName;

        if (categoryName == null) return StatusCode(400);
        if (categoryName.Length > 30) return StatusCode(400);
        
        var category = new ExamCategoryModel() {
            CategoryName = categoryName,
            UserId = HttpContext.Session.GetInt32("UserId")!.Value
        };
        
        try {
            _dbContext.ExamCategories.Add(category);
            await _dbContext.SaveChangesAsync();
        } catch (Exception E) {
            return StatusCode(500);
        }

        return StatusCode(200);
    }

    [ExamIdValidator]
    [HttpPost]
    public async Task<IActionResult> AddQuestion(AddQuestionModel model, int id = -1)
    {
        Console.WriteLine(model.QuestionText);
        Console.WriteLine(model.Type);
        foreach (var answer in model.Answers)
        {
            Console.WriteLine(answer.Content, answer.IsCorrect);
        }

        var question = new QuestionModel()
        {
            QuestionText = model.QuestionText,
            Type = model.Type,
        };

        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync();

        int qId = question.Id;
        
        foreach (var answer in model.Answers)
        {
            var answerEntity = new AnswerModel()
            {
                ExamId = answer.ExamId,
                QuestionId = qId,
                Content = answer.Content,
                IsCorrect = answer.IsCorrect
            };
            
            _dbContext.Answers.Add(answerEntity);
        }

        await _dbContext.SaveChangesAsync();

        return View("EditExam");
    }

    [ExamIdValidator]
    [Route("/Dashboard/Exam/{id:int}/Edit")]
    public async Task<IActionResult> EditExam(int id = -1) {
        var test = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        
        if (test.Count <= 0) {
            return RedirectToAction("Login", "Account");
        }
 
        var questions = await _dbContext.Questions.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.questions = questions;
        var answers = await _dbContext.Answers.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.answers = answers;

        
        return View();
    }
    
    

}