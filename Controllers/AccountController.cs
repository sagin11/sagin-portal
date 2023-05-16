﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using SaginPortal.Context;
using SaginPortal.Models;

namespace SaginPortal.Controllers; 

public class AccountController : Controller {

    private readonly AppDbContext _appDbContext;

    public AccountController(AppDbContext appDbContext) {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    public IActionResult Login() {
        
        if (HttpContext.Session.Keys.Contains("UserId")) {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model) {
        var password = model.Password;
        var email = model.Email;

        if (password == null || email == null)
            return View();
        var test = await _appDbContext.User.Take(1).ToListAsync();

        Console.WriteLine(test.FirstOrDefault());
        

        var results = await _appDbContext.User.Where(u => u.Email == email).Select(u => new {
            u.Email,
            u.PasswordHash,
            u.Id
        }).ToListAsync();

        if (results.Count > 0) {
            

            if (BCrypt.Net.BCrypt.Verify(password, results.FirstOrDefault()?.PasswordHash)) {
                HttpContext.Session.SetInt32("UserId", results.FirstOrDefault()!.Id);
                return RedirectToAction("Index", "Dashboard");
            } else {
                ModelState.AddModelError("Password", "Błędne hasło.");
            }
        } else {
            ModelState.AddModelError("Email", "Nie znaleziono użytkownika o podanym adresie Email.");
        }
        

        return View();
    }

    // public IActionResult Register() {
    //     return View();
    // }

    public IActionResult Logout() {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
    
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