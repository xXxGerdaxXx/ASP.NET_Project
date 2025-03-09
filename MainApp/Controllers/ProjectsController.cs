using MainApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;
[Route("projects")]

public class ProjectsController : Controller
{
    [Route("")]

    public IActionResult Projects()
    {
        return View(); 
    }

    //[Route("add")]
    //public IActionResult AddProject()
    //{
    //    return View();
    //}
}


