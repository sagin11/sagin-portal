using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;

namespace SaginPortal.Packages; 

public class ExamExistsValidatorAttribute : ActionFilterAttribute {
    private readonly AppDbContext _dbContext;

    public ExamExistsValidatorAttribute(AppDbContext dbContext) {
        _dbContext = dbContext;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        if (context.ActionArguments.ContainsKey("id")) {
            var id = (int)context.ActionArguments["id"];

            if (id < 0) {
                context.Result = new StatusCodeResult(404);
                return;
            }
        }
        
        var test = await _dbContext.Exams.Where(t => t.CreatorId == context.HttpContext.Session.GetInt32("UserId") && t.Id == (int)context.ActionArguments["id"]!).ToListAsync();
        
        if (test.Count <= 0) {
            var controller = context.Controller as Controller;
            
            context.Result = controller.RedirectToAction("Login", "Account");
            return;
        }
        await next();
    }
}