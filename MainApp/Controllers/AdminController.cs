using Microsoft.AspNetCore.Mvc;
using MainApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace MainApp.Controllers;
public class AdminController : Controller
{
    [AllowAnonymous]
    public IActionResult AdminSignIn()
    {
        return View();
    }

    public IActionResult Projects()
    {
        return View();
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Members()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Clients()
    {
        return View();
    }
}
