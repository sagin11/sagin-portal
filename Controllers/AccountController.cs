using Microsoft.AspNetCore.Mvc;
using SaginPortal.Models;

namespace SaginPortal.Controllers; 

public class AccountController : Controller {
    
    [HttpGet]
    public IActionResult Login() {

        return View();
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginModel model) {
        var password = model.Password;
        var email = model.Email;
        
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Password: {password}");
        
        return View();
    }

    // public IActionResult Register() {
    //     return View();
    // }

    // public IActionResult ForgotPassword() {
    //     return View();
    // }
    //
    // public IActionResult ResetPassword() {
    //     return View();
    // }
    //
    // public IActionResult AccessDenied() {
    //     return View();
    // }
}