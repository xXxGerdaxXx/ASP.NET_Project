using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Infrastructure.DTOs;
using Infrastructure.Helpers;

namespace MainApp.Controllers;

[Route("projects")]
public class ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger, FileService fileService, IClientService clientService, IMemberService memberService, IStatusService statusService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly ILogger<ProjectsController> _logger = logger;
    private readonly FileService _fileService = fileService;
    private readonly IClientService _clientService = clientService;
    private readonly IMemberService _memberService = memberService;
    private readonly IStatusService _statusService = statusService;

    [HttpGet("list")]
    public async Task<IActionResult> GetProjectsList()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        var viewModels = projects.Select(project => new ProjectViewModel
        {
            Id = project.Id,
            AvatarUrl = project.AvatarUrl, 
            Name = project.ProjectName,
            Company = project.Client?.ClientName ?? "Unknown",
            Description = project.Description,
            Status = project.Status?.StatusName ?? "N/A",
            Deadline = DateHelper.FormatDeadline(project.EndDate),
            TeamMembers = project.ProjectMembers.Select(pm => new TeamMember
            {
                Name = pm.Member.FirstName + " " + pm.Member.LastName,
                AvatarUrl = string.IsNullOrWhiteSpace(pm.Member.AvatarUrl) ? "/images/avatar.svg" : pm.Member.AvatarUrl
            }).ToList()
        }).ToList();

        if (projects == null || !projects.Any())
        {
            _logger.LogWarning(projects == null ? "Projects list is NULL!" : "Projects list is EMPTY!");
            projects = [];
        }
        else
        {
            _logger.LogInformation($"Retrieved {projects.Count} projects from the database.");
        }

        return PartialView("Partials/Sections/_ProjectTableBody", viewModels);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllProjectsAsync();

        var viewModels = projects.Select(project => new ProjectViewModel
        {
            Id = project.Id,
            Name = project.ProjectName,
            Company = project.Client?.ClientName ?? "Unknown",
            Description = project.Description,
            Status = project.Status?.StatusName ?? "N/A",
            Deadline = DateHelper.FormatDeadline(project.EndDate),
            TeamMembers = project.ProjectMembers.Select(pm => new TeamMember
            {
                Name = pm.Member.FirstName + " " + pm.Member.LastName,
                AvatarUrl = string.IsNullOrWhiteSpace(pm.Member.AvatarUrl) ? "/images/avatar.svg" : pm.Member.AvatarUrl
            }).ToList()
        }).ToList();

        return View(viewModels); 
    }


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

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var clients = await _clientService.GetAllClientsAsync();
        var statuses = await _statusService.GetAllAsync();
        var members = await _memberService.GetAllMembersAsync();

        var model = new ProjectCreateFormModel
        {
            ClientList = new SelectList(clients, "Id", "ClientName"),
            StatusList = new SelectList(statuses, "Id", "StatusName"),
            TeamMemberList = members.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.LastName}"
            }).ToList()
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
        if (form.ProjectImage is not null)
        {
            avatarUrl = await _fileService.SaveFileAsync(form.ProjectImage, "projects");
        }

        var dto = new ProjectDTO
        {
            ProjectName = form.Name,
            Description = form.Description,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            Budget = form.Budget,
            TeamMemberIds = form.SelectedTeamMemberIds,
            ClientId = form.ClientId,
            StatusId = form.StatusId,
            AvatarUrl = avatarUrl,
            CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system"
        };

        try
        {
            await _projectService.CreateProjectAsync(dto);
            _logger.LogInformation("Project Created Successfully: {@DTO}", dto);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Creating Project: {@DTO}", dto);
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

        var allMembers = await _memberService.GetAllMembersAsync();

        var model = new ProjectEditFormModel
        {
            Id = project.Id,
            Name = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            StatusId = project.StatusId,
            ClientId = project.ClientId,
            AvatarUrl = string.IsNullOrWhiteSpace(project.AvatarUrl) ? "/images/Avatar.svg" : project.AvatarUrl,
            ClientList = new SelectList(clients, "Id", "ClientName"),
            StatusList = new SelectList(statuses, "Id", "StatusName"),
            TeamMemberList = allMembers.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.LastName}",
                Selected = project.ProjectMembers.Any(pm => pm.MemberId == m.Id) // pre-select members
            }).ToList(),
            SelectedTeamMemberIds = project.ProjectMembers.Select(pm => pm.MemberId).ToList()
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
            project.ClientId = form.ClientId;
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
