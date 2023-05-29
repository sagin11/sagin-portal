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
        var model = new AddQuestionModel();
        model.Answers = new List<AnswerModel>();
        return View(model);
    }
    
    // TODO: Zmienić AddQuestionPost na AddQuestion w widoku
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [HttpPost]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/AddQuestionPost")]
    public async Task<IActionResult> AddQuestionPost(AddQuestionModel model, int id = -1) {
        
        // TODO: Sprawdzanie czy jakakolwiek odpowiedź jest zaznaczona jako poprawna
        // TODO: Rework całej metody bo nie działa xd
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
        
        var qId = question.Id;
        
        foreach (var answerEntity in answersList.Select(answer => new AnswerModel() {
                     ExamId = id,
                     QuestionId = qId,
                     Content = answer.Content,
                     IsCorrect = Convert.ToBoolean(answer.IsCorrect)
                 })) {
            _dbContext.Answers.Add(answerEntity);
        }
        
        await _dbContext.SaveChangesAsync();

        return Redirect("/Dashboard/Exam/" + id + "/Edit");
    }   

    
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/")]
    public async Task<IActionResult> Questions(int id = -1) {
        var questions = await _dbContext.Questions.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.questions = questions;
        var answers = await _dbContext.Answers.Where(q => q.ExamId == id).ToListAsync();
        ViewBag.answers = answers;

        return View();
    }
    
    [HttpGet]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/Delete/{questionId:int}")]
    public async Task<IActionResult> DeleteQuestion(int id = -1, int questionId = -1) {
        var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
        var answers = await _dbContext.Answers.Where(a => a.QuestionId == questionId).ToListAsync();
        _dbContext.Questions.Remove(question);
        foreach (var answer in answers) {
            _dbContext.Answers.Remove(answer);
        }
        await _dbContext.SaveChangesAsync();
        return Redirect($"/Dashboard/Exam/{id}/Edit/Questions");
    }

    [HttpGet]
    [CheckLoginStatus]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/Edit/{questionId:int}")]
    public async Task<IActionResult> EditQuestion(int id = -1, int questionId = -1) {
        QuestionModel? question;
        try {
            question = await _dbContext.Questions.FirstAsync(q => q.Id == questionId);
        } catch (Exception e) {
            return NotFound("No tego typu chcesz edytować coś co nie istnieje tak jak twoja inteligencja.");
        }
        var answers = await _dbContext.Answers.Where(a => a.QuestionId == questionId).ToListAsync();
        var model = new AddQuestionModel() {
            QuestionText = question.QuestionText,
            Type = question.Type,
            Answers = answers
        };
        ViewBag.ExamId = id;
        ViewBag.question = question;
        ViewBag.answers = answers;
        return View(model);
    }

    [HttpPost]
    [CheckLoginStatus]
    [ServiceFilter(typeof(ExamExistsValidatorAttribute))]
    [Route("/Dashboard/Exam/{id:int}/Edit/Questions/Edit/{questionId:int}")]
    public async Task<IActionResult> EditQuestion(AddQuestionModel model, int id = -1, int questionId = -1) {
        QuestionModel? question;
        try {
            question = await _dbContext.Questions.FirstAsync(q => q.Id == questionId);
        } catch (Exception e) {
            return NotFound("No tego typu chcesz edytować coś co nie istnieje tak jak twoja inteligencja.");
        }
        var answersList = model.Answers;
        foreach (var answer in answersList) {
            if (answer.Content == "" || answer.Content == null) return StatusCode(400);           
        }
        if (string.IsNullOrEmpty(model.QuestionText)) return StatusCode(400);
        
        question!.QuestionText = model.QuestionText;
        question.Type = model.Type;
        
        _dbContext.Questions.Update(question);
        
        var existingAnswers = await _dbContext.Answers.Where(a => a.QuestionId == question.Id).ToListAsync();
    
        if (existingAnswers.Count > model.Answers.Count) {
            var answersToDelete = existingAnswers.Where(a => model.Answers.All(ma => ma.Content != a.Content));
            foreach (var answer in answersToDelete) {
                _dbContext.Answers.Remove(answer);
            }
        } else {
            var existingAnswersList = await _dbContext.Answers.Where(a => a.ExamId == question.ExamId).ToListAsync();
            foreach (var answerModel in model.Answers) {
                foreach (var existingAnswer in existingAnswersList) {
                    if (existingAnswer != null) {
                        existingAnswer.Content = answerModel.Content;
                        existingAnswer.IsCorrect = answerModel.IsCorrect;
                        _dbContext.Answers.Update(existingAnswer);
                    } else {
                        var newAnswer = new AnswerModel {
                            Content = answerModel.Content,
                            IsCorrect = answerModel.IsCorrect,
                            QuestionId = question.Id,
                            ExamId = question.ExamId
                        };
                        _dbContext.Answers.Add(newAnswer);
                    }
                }
            }  
        }
        
        await _dbContext.SaveChangesAsync();
        
        
        // SPRAWDZAĆ CZY W model.Answers jest więcej odpowiedzi niż w existingAnswers jeżeli jest więcej no to dodajemy jeżeli nie to nadpisujemy
        // USUWANIE ODPOWIEDZI W JAVASCRIPT JAKO JAKIEŚ API I REFRESHOWAĆ STAN PYTAŃ I ODPOWIEDZI
        
        // foreach (var answerModel in model.Answers) {
        //     var existingAnswer = existingAnswers.FirstOrDefault(a => a.Content == answerModel.Content);
        //     if (existingAnswer != null) {
        //         existingAnswer.Content = answerModel.Content;
        //         existingAnswer.IsCorrect = answerModel.IsCorrect;
        //         _dbContext.Answers.Update(existingAnswer);
        //     } else {
        //         var newAnswer = new AnswerModel {
        //             Content = answerModel.Content,
        //             IsCorrect = answerModel.IsCorrect,
        //             QuestionId = question.Id,
        //             ExamId = question.ExamId
        //         };
        //         _dbContext.Answers.Add(newAnswer);
        //     }
        // }

        await _dbContext.SaveChangesAsync();

        return Redirect("/Dashboard/Exam/" + id + "/Edit");
    }

}