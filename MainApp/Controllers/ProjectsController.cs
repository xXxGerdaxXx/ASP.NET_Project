using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MainApp.Controllers;



[Route("projects")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;
    private readonly FileService _fileService;
    private readonly IClientService _clientService;
    private readonly IMemberService _memberService;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger, FileService fileService, IClientService clientService, IMemberService memberService)
    {
        _projectService = projectService;
        _logger = logger;
        _fileService = fileService;
        _clientService = clientService;
        _memberService = memberService;
        _memberService = memberService;
    }
    // Handle Avatar Uploads
    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadProjectAvatar(IFormFile file)
    {
        string? fileUrl = await _fileService.SaveFileAsync(file, "projects"); 
        if (fileUrl == null)
        {
            return BadRequest("Error uploading file.");
        }

        return Ok(new { url = fileUrl });
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
        return PartialView("Partials/Sections/_CreateProject");

    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateProject(ProjectCreateFormModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            _logger.LogWarning("Form validation failed: {@Errors}", errors);
            return BadRequest(new { success = false, errors });
        }

        //string? avatarUrl = null;
        //if (model.ProjectImage != null)
        //{
        //    avatarUrl = await _fileService.SaveFileAsync(model.ProjectImage, "projects");
        //}



        var newProject = new ProjectEntity
        {
            ProjectName = model.Name,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            //ClientId = model.ClientId,
            CreatedByUserId = User.Identity?.Name ?? "system", // Or however you store user IDs
            StatusId = 1 // Default to a "started" status, or adjust accordingly
        };

        try
        {
            await _projectService.CreateProjectAsync(newProject);
            _logger.LogInformation("Project Created Successfully: {@Project}", newProject);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Creating Project: {@Form}", model);
            return StatusCode(500, new { success = false, message = "Internal Server Error" });
        }
    }
}
