using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;
using SaginPortal.Packages;

namespace SaginPortal.Controllers; 

public class QuestionsController : Controller {
    private readonly AppDbContext _dbContext;

    public QuestionsController(AppDbContext dbContext) {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/AddQuestion")]
    public async Task<IActionResult> AddQuestion(int id = -1) {
        ViewBag.ExamId = id;
        return View();
    }
    
    // TODO: Zmienić AddQuestionPost na AddQuestion w widoku
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [HttpPost]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/AddQuestionPost")]
    public async Task<IActionResult> AddQuestion(AddQuestionModel model, int id = -1) {
        var answersList = model.Answers;
        foreach (var answer in answersList) {
            if (answer.Content == "" || answer.Content == null) return BadRequest("Puste pole odpowiedzi!");           
        }

        // TODO: Sprawdzanie czy jakakolwiek odpowiedź jest poprawna
        
        // bool isAnyCorrect = answersList.Any(answer => answer.IsCorrect);
        //
        // if (!isAnyCorrect) {
        //     return BadRequest("No correct answer was provided!");
        // }
        //
        // return Ok();

        if (string.IsNullOrEmpty(model.QuestionText)) return StatusCode(400);

        var question = new QuestionModel() {
            QuestionText = model.QuestionText,
            Type = model.Type,
            ExamId = id
        };

        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync();
        
        var qId = question.Id;
        
        foreach (var answerEntity in answersList.Select(answer => new AnswerModel() {
                     ExamId = id,
                     QuestionId = qId,
                     Content = answer.Content,
                     IsCorrect = Convert.ToBoolean(answer.IsCorrect) 
                 }))
        {
            _dbContext.Answers.Add(answerEntity);
        }
        
        await _dbContext.SaveChangesAsync();
        
        return Redirect($"/Dashboard/Exam/{id}/Edit/");
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
}