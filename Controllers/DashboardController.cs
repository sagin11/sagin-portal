using Microsoft.AspNetCore.Mvc;
using SaginPortal.Context;

namespace SaginPortal.Controllers; 

public class DashboardController : Controller {
    private readonly AppDbContext _appDbContext;

    public DashboardController(AppDbContext appDbContext) {
        _appDbContext = appDbContext;
    }
    
    public IActionResult Index() {
        if (HttpContext.Session.Keys.Contains("UserId")) {
            return View();
        }

        return RedirectToAction("Login", "Account");
    }
    

}