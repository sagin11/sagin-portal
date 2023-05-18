using Microsoft.EntityFrameworkCore;
using SaginPortal.Models;
using SaginPortal.Models.ExamModels;

namespace SaginPortal.Context;

public class AppDbContext : DbContext {
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        
    }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<AnswerModel> Answers { get; set; }
    public DbSet<QuestionModel> Questions { get; set; }
    // public DbSet<AddQuestionModel> AddQuestions { get; set; }
    public DbSet<ExamModel> Exams { get; set; }
    public DbSet<ExamCategoryModel> ExamCategories { get; set; }
    

}