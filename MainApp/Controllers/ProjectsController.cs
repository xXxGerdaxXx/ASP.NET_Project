using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;



[Route("projects")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;
    private readonly FileService _fileService;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger, FileService fileService)
    {
        _projectService = projectService;
        _logger = logger;
        _fileService = fileService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetProjectsList()
    {
        var projects = await _projectService.GetAllProjectsAsync();

        if (projects == null || !projects.Any())
        {
            _logger.LogWarning(projects == null ? "Projects list is NULL!" : "Projects list is EMPTY!");
            projects = new List<ProjectEntity>();
        }
        else
        {
            _logger.LogInformation($"Retrieved {projects.Count} projects from the database.");
        }

        return PartialView("Partials/Sections/_ProjectTableBody", projects);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return View(projects);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Shared/Partials/Sections/_CreateProject.cshtml", new ProjectCreateFormModel());

    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ProjectCreateFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("~/Views/Shared/Partials/Sections/_CreateProject.cshtml", new ProjectCreateFormModel());


        }

        var newProject = new ProjectEntity
        {
            ProjectName = model.Name,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = model.ClientId,
            CreatedByUserId = User.Identity?.Name ?? "system", // Or however you store user IDs
            StatusId = 1 // Default to a "started" status, or adjust accordingly
        };

        await _projectService.CreateProjectAsync(newProject);

        return Json(new { success = true });
    }
}
