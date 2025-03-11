using MainApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
    private static List<ProjectCreateFormModel> _projects = new(); // Temporary In-Memory Storage (Replace with DB Later)

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(_projects); // Show all projects
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        var model = new ProjectCreateFormModel
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(1) 
        };
        return PartialView("_Create", model); 
    }

    [HttpPost("create")]
    public IActionResult Create(ProjectCreateFormModel model)
    {
        if (!ModelState.IsValid)
            return PartialView("_Create", model); // Reload Modal with Errors

        _projects.Add(model); // Store Project (Replace with DB Logic)

        return RedirectToAction("Index"); // ✅ Redirect to Projects List
    }

    // ✅ Search Members API for Multi-Select (AJAX)
    [HttpGet("search-members")]
    public IActionResult SearchMembers(string query)
    {
        var members = new List<TeamMember>
        {
            new TeamMember { Name = "Alice Johnson", AvatarUrl = "/images/alice.png" },
            new TeamMember { Name = "Bob Smith", AvatarUrl = "/images/bob.png" },
            new TeamMember { Name = "Charlie Brown", AvatarUrl = "/images/charlie.png" }
        };

        var results = members
            .Where(m => m.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Json(results);
    }
}
