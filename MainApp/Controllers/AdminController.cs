using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;
[Route("admin")]

public class AdminController : Controller
{
    [Route("members")]
    public IActionResult Index()
    {
        return View();
    }
}
