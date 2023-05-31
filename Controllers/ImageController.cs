using Microsoft.AspNetCore.Mvc;
using SaginPortal.Context;
using SaginPortal.Models;
using SaginPortal.Packages;

namespace SaginPortal.Controllers; 

public class ImageController : Controller {
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _hostEnvironment;
    
    public ImageController(IWebHostEnvironment hostEnvironment, AppDbContext dbContext) {
        _hostEnvironment = hostEnvironment;
        _dbContext = dbContext;
    }

    [HttpPost]
    [CheckLoginStatus]
    public async Task<IActionResult> Upload() {
        var file = Request.Form.Files[0];

        if (file.Length > 0 && file.Length <= 3145728) {
            try {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try {
                    var image = new UploadedImageModel() {
                        OriginalFileName = file.FileName,
                        FileName = uniqueFileName,
                        FilePath = filePath,
                        UploadDate = DateTime.Now,
                        OwnerId = HttpContext.Session.GetInt32("UserId")
                    };

                    _dbContext.Add(image);
                    await _dbContext.SaveChangesAsync();                
                } catch (Exception e) { 
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                
                
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                
                
                var baseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppUrls").Value;

                return Json(new {location = baseUrl + "uploads/" + uniqueFileName});
                
            } catch (Exception e) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        } else {
            return Json(new {error = "No file found in request or file is too big (3MB)."});
        }
    }
    
    // TODO: Add DeleteImage method
}