using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;

namespace SaginPortal.Controllers; 

public class DashboardController : Controller {
    private readonly AppDbContext _dbContext;

    public DashboardController(AppDbContext appDbContext) {
        _dbContext = appDbContext;
    }
    
    public async Task<IActionResult> Index() {
        
        if (!HttpContext.Session.Keys.Contains("UserId")) {
            return RedirectToAction("Login", "Account");

        }
        
        var tests = await _dbContext.Tests.Where(t => t.CreatorId == HttpContext.Session.GetInt32("UserId")).ToListAsync();
        
        if (!(tests.Count > 0)) return View();
        
        var questions = await _dbContext.Questions.ToListAsync();
        var answers = await _dbContext.Answers.ToListAsync();
        ViewBag.tests = tests;
        ViewBag.questions = questions;
        ViewBag.answers = answers;

        return View();
    }
    

}