using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        var exams = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId"))
            .Join(
                _dbContext.ExamCategories,
                exam => exam.CategoryId,
                category => category.Id,
                (exam, category) => new { Exam = exam, Category = category }
            )
            .ToListAsync();

        if (!(exams.Count > 0)) return View();

        var questions = await _dbContext.Questions.ToListAsync();
        var answers = await _dbContext.Answers.ToListAsync();
        var categories = await _dbContext.ExamCategories.ToListAsync();
        ViewBag.categories = categories;
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;

        return View();
    }
    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("Dashboard/Exam/{id:int}")]
    public async Task<IActionResult> Exam(int id = -1) {

        var test = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        
        if (test.Count <= 0) {
            return RedirectToAction("Login", "Account");
        }
        
        var exams = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        var questions = await _dbContext.Questions.Where(t => t.ExamId == id).ToListAsync();
        var answers = await _dbContext.Answers.Where(t => t.ExamId == id).ToListAsync();
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;
        ViewBag.ExamId = id;
        return View();
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
    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit")]
    public async Task<IActionResult> EditExam(int id = -1) {
        var test = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        
        if (test.Count <= 0) {
            return RedirectToAction("Login", "Account");
        }
        
        HttpContext.Session.SetInt32("ExamId", id);
 
        var questions = await _dbContext.Questions.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.questions = questions;
        var answers = await _dbContext.Answers.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.answers = answers;
        
        return View();
        
    }
    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/AddQuestion")]
    public async Task<IActionResult> AddQuestion(int id = -1) {
        ViewBag.ExamId = id;
        return View();
    }
    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/")]
    public async Task<IActionResult> Questions(int id = -1)
    {
        var questions = await _dbContext.Questions.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.questions = questions;
        var answers = await _dbContext.Answers.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.answers = answers;

        return View();
    }
    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/QuestionsSet")]
    public async Task<IActionResult> QuestionsSet(int id = -1) {
        var examConfiguration = await _dbContext.ExamConfigurationModels.Where(e => e.ExamId == id).FirstOrDefaultAsync();
        var questionsCount = await _dbContext.Questions.Where(q => q.ExamId == id).CountAsync();

        ViewBag.questionsCount = questionsCount;
        ViewBag.configuration = examConfiguration!;
        return View();
    }


}