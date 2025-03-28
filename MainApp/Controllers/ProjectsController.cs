using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    private readonly IStatusService _statusService;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger, FileService fileService, IClientService clientService, IMemberService memberService, IStatusService statusService)
    {
        _projectService = projectService;
        _logger = logger;
        _fileService = fileService;
        _clientService = clientService;
        _memberService = memberService;
        _statusService = statusService;
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

    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadProjectAvatar(IFormFile file)
    {
        string? fileUrl = await _fileService.SaveFileAsync(file, "projects"); //  Use FileService
        if (fileUrl == null)
        {
            return BadRequest("Error uploading file.");
        }

        return Ok(new { url = fileUrl });
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var clients = await _clientService.GetAllClientsAsync();
        var statuses = await _statusService.GetAllAsync();

        var model = new ProjectCreateFormModel
        {
            ClientList = new SelectList(clients, "Id", "ClientName"),
            StatusList = new SelectList(statuses, "Id", "StatusName")
        };

        return PartialView("Partials/Sections/_CreateProject", model);
    }



    [HttpPost("create")]
    public async Task<IActionResult> CreateProject(ProjectCreateFormModel form)
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

        string? avatarUrl = null;
        if (form.ProjectImage != null)
        {
            avatarUrl = await _fileService.SaveFileAsync(form.ProjectImage, "projects");
        }

        var newProject = new ProjectEntity
        {
            ProjectName = form.Name,
            Description = form.Description,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            Budget = form.Budget,
            ClientId = form.ClientId,
            CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system",
            StatusId = form.StatusId,
            AvatarUrl = avatarUrl
        };

        try
        {
            await _projectService.CreateProjectAsync(newProject);
            _logger.LogInformation("Project Created Successfully: {@Project}", newProject);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Creating Project: {@Form}", form);
            return StatusCode(500, new { success = false, message = "Internal Server Error" });
        }
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        var statuses = await _statusService.GetAllAsync();
        var clients = await _clientService.GetAllClientsAsync();

        if (project == null) return NotFound();



        var model = new ProjectEditFormModel
        {
            Id = project.Id,
            Name = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            StatusId = project.StatusId, 
            StatusList = new SelectList(statuses, "Id", "StatusName"),
            ClientList = new SelectList(clients, "Id", "ClientName")

        };

        return PartialView("~/Views/Shared/Partials/Sections/_EditProject.cshtml", model);
    }


    [HttpPost("editproject")]
    public async Task<IActionResult> Edit(ProjectEditFormModel form)
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

        try
        {
            var project = await _projectService.GetProjectByIdAsync(form.Id);
            if (project == null) return NotFound();

            if (form.ProjectImage != null)
            {
                var uploadedFilePath = await _fileService.SaveFileAsync(form.ProjectImage, "projects");
                if (!string.IsNullOrEmpty(uploadedFilePath))
                {
                    project.AvatarUrl = uploadedFilePath;
                }
            }

            project.ProjectName = form.Name;
            project.Description = form.Description;
            project.StartDate = form.StartDate;
            project.EndDate = form.EndDate;
            project.Budget = form.Budget;
            project.StatusId = form.StatusId;

            await _projectService.UpdateProjectAsync(project);
            _logger.LogInformation("Project updated successfully: {@Project}", project);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project: {@Form}", form);
            return StatusCode(500, new { success = false, message = "Error updating project." });
        }
    }


    [HttpPost("delete/{projectId}")]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        if (projectId <= 0)
        {
            return BadRequest(new { success = false, message = "Invalid project ID." });
        }

        try
        {
            bool deleted = await _projectService.DeleteProjectAsync(projectId);
            if (!deleted)
            {
                return NotFound(new { success = false, message = "Project not found or could not be deleted." });
            }

            return Json(new { success = true, message = "Project deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project ID {ProjectId}", projectId);
            return StatusCode(500, new { success = false, message = "Error deleting the project." });
        }
    }

}
