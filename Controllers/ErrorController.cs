using Microsoft.AspNetCore.Mvc;

namespace SaginPortal.Controllers;

public class ErrorController : Controller
{
    [Route("/Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewBag.statuscode = statusCode;
        return View("Index");
    }

}