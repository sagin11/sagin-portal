using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;
using SaginPortal.Packages;

namespace SaginPortal.Controllers;

// TODO: Sprawdzanie czy test jest aktywny, jeżeli jest to wtedy nie można dodawać pytań.
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
        var results = await _dbContext.Results.Where(r => r.ExamId == id).ToListAsync();
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;
        ViewBag.ExamId = id;
        ViewBag.startedTests = results.Count;
        //TODO Zdawalność ile % wymaga  żeby zdać

        double points = 0;
        int maxPoints = 0; 

        foreach (var result in results)
        {
            points += result.Points;
            maxPoints += result.MaxPoints;
        }
        ViewBag.avgScore = Math.Round(points / maxPoints * 100, 0);
        
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

            var baseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppUrls").Value;

            while (true) {
                try {
                    _dbContext.ExamConfiguration.Add(new ExamConfigurationModel() {
                        ExamId = exam.Id,
                        ExamUrl = $"{baseUrl}Exam/{new RandomStringGenerator().GenerateRandomString()}"
                    });
                    await _dbContext.SaveChangesAsync();
                    break;
                }
                catch (Exception e) {
                    Console.WriteLine("Exception: " + e.Message);
                }        
            }
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

    [HttpGet]
    [CheckLoginStatus]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/AccessSettings")]
    public async Task<IActionResult> AccessSettings(int id = -1) {
        var exam = await _dbContext.Exams
            .Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();

        var examConfiguration = await _dbContext.ExamConfiguration.Where(e => e.ExamId == id).FirstOrDefaultAsync();

        if (examConfiguration == null) {
            return StatusCode(403);
        }
        
        ViewBag.examUrl = examConfiguration.ExamUrl;
        ViewBag.exam = exam;
        
        
        return View();
    }

    [HttpGet]
    [CheckLoginStatus]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Activate")]
    public async Task<IActionResult> EnableTest(int id = -1) {
        var exam = _dbContext.Exams.Where(e => e.Id == id).FirstOrDefault();
        
        if (exam == null) {
            return StatusCode(403);
        }
        
        exam.Status = "Active";
        _dbContext.Exams.Update(exam);
        await _dbContext.SaveChangesAsync();
        
        return RedirectToAction("ExamDetails", "ExamManager", new {id});
    }
    
}