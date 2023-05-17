using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SaginPortal.Packages; 

public class CheckLoginStatusAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        if (!session.Keys.Contains("UserId"))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
    }

}