using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;
using SaginPortal.Packages;

namespace SaginPortal.Controllers;

public class ExamManagerController : Controller {
    private readonly AppDbContext _dbContext;

    public ExamManagerController(AppDbContext dbContext) {
        _dbContext = dbContext;
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
    [HttpPost]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/AddQuestionPost")]
    public async Task<IActionResult> AddQuestionPost(AddQuestionModel model, int id = -1) {
        var answersList = model.Answers;
        foreach (var answer in answersList) {
            if (answer.Content == "" || answer.Content == null) return StatusCode(400);           
        }

        if (string.IsNullOrEmpty(model.QuestionText)) return StatusCode(400);

        
        var question = new QuestionModel() {
            QuestionText = model.QuestionText,
            Type = model.Type,
            ExamId = id
        };
        
        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync();
        
        var qId = question.Id; // Pobierz prawidłowy identyfikator pytania
        
        foreach (var answerEntity in answersList.Select(answer => new AnswerModel()
                 {
                     ExamId = id,
                     QuestionId = qId,
                     Content = answer.Content,
                     IsCorrect = Convert.ToBoolean(answer.IsCorrect)
                 }))
        {
            _dbContext.Answers.Add(answerEntity);
        }
        
        await _dbContext.SaveChangesAsync();

        // return Ok();
        return Redirect("/Dashboard/Exam/" + id + "/Edit/");
    }
    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/QuestionsSet")]
    [HttpPost]
    public async Task<IActionResult> QuestionsSet(ExamConfigurationModel model, int id = -1) {
        var randomizeQuestions = model.RandomizeQuestions;
        var questionTime = model.QuestionTime;

        var examConfiguration = await _dbContext.ExamConfigurationModels.Where(e => e.ExamId == id).FirstOrDefaultAsync();

        examConfiguration.RandomizeQuestions = randomizeQuestions;
        examConfiguration.QuestionTime = questionTime;

        await _dbContext.SaveChangesAsync();
        
        ViewBag.configuration = examConfiguration!;
        return View($"~/Views/Dashboard/QuestionsSet.cshtml");
    }

    
}