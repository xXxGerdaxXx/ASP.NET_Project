using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[Authorize(Roles = "Admin, User")]

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
