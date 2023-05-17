using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SaginPortal.Packages; 

public class ExamIdValidatorAttribute : ActionFilterAttribute {
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        if (context.ActionArguments.ContainsKey("id")) {
            var id = (int)context.ActionArguments["id"];

            if (id < 0) {
                context.Result = new StatusCodeResult(404);
                return;
            }
        }
        await next();
    }
}