using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;

namespace SaginPortal.Packages; 

public class ShuffleList {
    private readonly AppDbContext _dbContext;

    public ShuffleList(AppDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<List<QuestionModel>> Shuffle(int examId) {
        var questions = await _dbContext.Questions.Where(q => q.ExamId == examId).ToListAsync();

        var random = new Random();
        var shuffledQuestions = questions.OrderBy(q => random.Next()).ToList();

        return shuffledQuestions;
    }
}