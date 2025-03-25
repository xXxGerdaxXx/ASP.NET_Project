using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[Authorize(Roles = "User,Admin")]

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
