using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;
using SaginPortal.Packages;

namespace SaginPortal.Controllers;

public class ExamManagerController : Controller {
    private readonly AppDbContext _dbContext;

    public ExamManagerController(AppDbContext dbContext, IWebHostEnvironment hostEnvironment) {
        _dbContext = dbContext;
    }
    
        
    [HttpGet]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("Dashboard/Exam/{id:int}")]
    public async Task<IActionResult> ExamDetails(int id = -1) {

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

    [HttpPost]
    [Route("Dashboard/CreateExam")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExam(AddExamModel model) {
        var examName = model.Name;
        var examCategoryId = model.CategoryId;
        var examCategoryName = model.CategoryName;

        if (examName == null) return RedirectToAction("Index", "Dashboard");
        
        if (examCategoryName == null) {

            var exam = new ExamModel() {
                Name = examName,
                CreatorId = HttpContext.Session.GetInt32("UserId")!.Value,
                CategoryId = examCategoryId,
                CreationTime = DateTime.Now,
                Status = "Disabled"
            };
            _dbContext.Exams.Add(exam);
            await _dbContext.SaveChangesAsync();
            
            _dbContext.ExamConfigurationModels.Add(new ExamConfigurationModel() {
                ExamId = exam.Id,
            });
            
            await _dbContext.SaveChangesAsync();
        }
        else {
            var examCategory = new ExamCategoryModel() {
                CategoryName = examCategoryName,
                UserId = HttpContext.Session.GetInt32("UserId")!.Value
            };
            
            _dbContext.ExamCategories.Add(examCategory);
            await _dbContext.SaveChangesAsync();

            var exam = new ExamModel() {
                Name = examName,
                CreatorId = HttpContext.Session.GetInt32("UserId")!.Value,
                CategoryId = examCategory.Id,
                CreationTime = DateTime.Now,
                Status = "Disabled"
            };
            
            _dbContext.Exams.Add(exam);
            await _dbContext.SaveChangesAsync();
        }


        return RedirectToAction("Index", "Dashboard");
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
}