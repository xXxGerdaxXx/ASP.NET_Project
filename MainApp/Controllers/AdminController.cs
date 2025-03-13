using Microsoft.AspNetCore.Mvc;
using MainApp.Models;

namespace MainApp.Controllers;
[Route("admin")]

public class AdminController : Controller
{
    [Route("members")]
    public IActionResult Members()
    {
        return View();
    }

    [Route("clients")]

    public IActionResult Clients()
    {
        return View();
    }

    [HttpPost]

    public IActionResult CreateClient(ClientCreateFormModel form) 
    {
        if (!ModelState.IsValid)
       
           return RedirectToAction("Clients");
        
        return View();
    }
}
