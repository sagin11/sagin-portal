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
    [Route("/Dashboard/Exam/{id:int}/Edit/QuestionsSet")]
    public async Task<IActionResult> QuestionsSet(int id = -1) {
        var examConfiguration = await _dbContext.ExamConfiguration.Where(e => e.ExamId == id).FirstOrDefaultAsync();
        var questionsCount = await _dbContext.Questions.Where(q => q.ExamId == id).CountAsync();

        ViewBag.questionsCount = questionsCount;
        ViewBag.configuration = examConfiguration!;
        return View();
    }

        
    
    [ValidateAntiForgeryToken]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/QuestionsSet")]
    [HttpPost]
    public async Task<IActionResult> QuestionsSet(ExamConfigurationModel model, int id = -1) {
        var randomizeQuestions = model.RandomizeQuestions;
        var questionTime = model.QuestionTime;
        var questionsCount = model.QuestionCount;
        
        var examConfiguration = await _dbContext.ExamConfiguration.Where(e => e.ExamId == id).FirstOrDefaultAsync();

        examConfiguration!.RandomizeQuestions = randomizeQuestions;
        examConfiguration.QuestionTime = questionTime;

        if (randomizeQuestions) {
            examConfiguration.QuestionCount = questionsCount;
            // TODO: Wyświetlanie komunikatu o błędzie
        }

        await _dbContext.SaveChangesAsync();
        
        ViewBag.configuration = examConfiguration!;
        return View($"~/Views/Dashboard/QuestionsSet.cshtml");
    }
}