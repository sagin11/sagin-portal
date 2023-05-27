using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models.ExamModels;

namespace SaginPortal.Controllers; 

public class CategoryController : Controller {
    private readonly AppDbContext _dbContext;

    public CategoryController(AppDbContext dbContext) {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserCategories() {
        var categories = await _dbContext.ExamCategories.Where(c => c.UserId == HttpContext.Session.GetInt32("UserId")!.Value).Select(c => new {
            c.Id, 
            c.CategoryName
        }).ToListAsync();
        
        if (categories.Count <= 0) return Json(new { data = "No data." });
        
        return Json(new { data = categories});
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
}