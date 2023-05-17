using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Packages;

namespace SaginPortal.Controllers; 

[CheckLoginStatus]
public class DashboardController : Controller {
    private readonly AppDbContext _dbContext;

    public DashboardController(AppDbContext appDbContext) {
        _dbContext = appDbContext;
    }
    
    public async Task<IActionResult> Index() {

        var exams = await _dbContext.Exams.Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId")).ToListAsync();
        
        if (!(exams.Count > 0)) return View();
        
        var questions = await _dbContext.Questions.ToListAsync();
        var answers = await _dbContext.Answers.ToListAsync();
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;

        return View();
    }
    
    [Route("Dashboard/Exam/{id:int}")]
    public async Task<IActionResult> Exam(int id = -1) {

        var test = await _dbContext.Exams.Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();

        if (test.Count <= 0) {
            return RedirectToAction("Login", "Account");
        }
        
        Console.WriteLine(test.Count);
        
        if (id == -1) {
            return StatusCode(404);
        }
        
        var exams = await _dbContext.Exams.Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId") && t.Id == id).ToListAsync();
        var questions = await _dbContext.Questions.Where(t => t.ExamId == id).ToListAsync();
        var answers = await _dbContext.Answers.Where(t => t.ExamId == id).ToListAsync();
        ViewBag.exams = exams;
        ViewBag.questions = questions;
        ViewBag.answers = answers;
        return View();
    }
    
    
    [Route("Dashboard/CreateExam")]
    public async Task<IActionResult> CreateExam() {
        
        // _dbContext.Questions.Join(t => t.ExamId == 1);
        
        return View();
    }
    

}