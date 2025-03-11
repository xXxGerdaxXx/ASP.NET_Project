using MainApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        var projects = new List<ProjectViewModel>
    {
        new ProjectViewModel { Name = "Alpha CRM", Description = "A customer management tool.", Status = "started" },
        new ProjectViewModel { Name = "E-commerce App", Description = "Online shopping platform.", Status = "completed" }
    };

        return View(projects);
    }


    [HttpGet("create")]
    public IActionResult Create()
    {
        // ✅ Pass a SINGLE model, not a list!
        return PartialView("_Create", new ProjectCreateFormModel());
    }

    [HttpPost("create")]
    public IActionResult Create(ProjectCreateFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_Create", model); // ✅ Return same model if invalid
        }

        // TODO: Save project to database

        return Json(new { success = true });
    }
}
