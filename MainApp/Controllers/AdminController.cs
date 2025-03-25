using Microsoft.AspNetCore.Mvc;
using MainApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace MainApp.Controllers;
[Route("admin")]
[Authorize]
public class AdminController : Controller
{
    [Route("index")]
    public IActionResult Index()
    {
        return View();
    }
    [Route("projects")]
    public IActionResult Projects()
    {
        return View();
    }

    [Route("AdminSignIn")]
    [HttpGet]
    public IActionResult AdminSignIn()
    {
        return View();
    }
    [Authorize(Roles = "Admin")]
    [Route("members")]
    public IActionResult Members()
    {
        return View();
    }
    [Authorize(Roles = "Admin")]
    [Route("clients")]

    public IActionResult Clients()
    {
        return View();
    }
}