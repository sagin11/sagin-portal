using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SaginPortal.Context;
using SaginPortal.Models;

namespace SaginPortal.Controllers; 

public class ExamController : Controller {
    
    private readonly AppDbContext _dbContext;
    private readonly AppUrls _appUrls;

    public ExamController(AppDbContext dbContext, IOptions<AppUrls> appUrls) {
        _dbContext = dbContext;
        _appUrls = appUrls.Value;
    }

    [Route("/Exam/{examUrl}")]
    public async Task<IActionResult> Index(string examUrl = "") {
        var examConfiguration = await _dbContext.ExamConfiguration.FirstOrDefaultAsync(e => e.ExamUrl == $"{_appUrls.Url}Exam/{examUrl}");

        if (examConfiguration == null) {
            return NotFound();
        }
        
        return View();
    }
}