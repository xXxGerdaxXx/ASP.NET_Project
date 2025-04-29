using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using Infrastructure.DTOs;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace MainApp.Controllers;

[Route("projects")]
public class ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger, IFileService fileService, IClientService clientService, IMemberService memberService, IStatusService statusService, ICompositeViewEngine viewEngine, INotificationService notificationService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly ILogger<ProjectsController> _logger = logger;
    private readonly IFileService _fileService = fileService;
    private readonly IClientService _clientService = clientService;
    private readonly IMemberService _memberService = memberService;
    private readonly IStatusService _statusService = statusService;
    private readonly ICompositeViewEngine _viewEngine = viewEngine;
    private readonly INotificationService _notificationService = notificationService;


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
            EndDate = project.EndDate, 
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
            EndDate = project.EndDate, 
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
            ProjectMemberIds = form.SelectedTeamMemberIds,
            ClientId = form.ClientId ?? 0,
            StatusId = form.StatusId ?? 0,
            AvatarUrl = avatarUrl,
            CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system"
        };

        try
        {
            await _projectService.CreateProjectAsync(dto);
            _logger.LogInformation("Project Created Successfully: {@DTO}", dto);

            // Create a notification for users
            await _notificationService.AddNotificationAsync(new NotificationEntity
            {
                NotificationTypeId = 3, 
                Message = $"A new project '{dto.ProjectName}' has been created!",
                NotificationTargetGroupId = 2, 
                CreatedAt = DateTime.Now
            });

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
        var allMembers = await _memberService.GetAllMembersAsync();

        if (project == null)
            return NotFound();

        var preselected = project.ProjectMembers.Select(pm => new
        {
            id = pm.MemberId,
            tagName = $"{pm.Member.FirstName} {pm.Member.LastName}",
            avatar = string.IsNullOrWhiteSpace(pm.Member.AvatarUrl) ? "/images/avatar.svg" : pm.Member.AvatarUrl
        }).ToList();

        ViewBag.PreselectedTeamMembersJson = JsonSerializer.Serialize(preselected);

        var model = new ProjectEditFormModel
        {
            Id = project.Id,
            Name = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            StatusId = project.StatusId ?? 0,
            ClientId = project.ClientId ?? 0,
            AvatarUrl = string.IsNullOrWhiteSpace(project.AvatarUrl) ? "/images/Avatar.svg" : project.AvatarUrl,
            ClientList = new SelectList(clients, "Id", "ClientName"),
            StatusList = new SelectList(statuses, "Id", "StatusName"),

            TeamMemberList = allMembers.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.LastName}",
                Selected = project.ProjectMembers.Any(pm => pm.MemberId == m.Id)
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
            if (project == null)
                return NotFound();

            if (form.ProjectImage != null)
            {
                var uploadedFilePath = await _fileService.SaveFileAsync(form.ProjectImage, "projects");
                if (!string.IsNullOrEmpty(uploadedFilePath))
                {
                    project.AvatarUrl = uploadedFilePath;
                }
            }

            var updateDto = new ProjectUpdateDTO
            {
                Id = project.Id,
                ProjectName = form.Name,
                Description = form.Description,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                Budget = form.Budget,
                ClientId = form.ClientId ?? 0,
                StatusId = form.StatusId ?? 0,
                AvatarUrl = project.AvatarUrl, 
                TeamMemberIds = form.SelectedTeamMemberIds,
                CreatedByUserId = project.CreatedByUserId
            };

            bool success = await _projectService.UpdateProjectAsync(updateDto);
            if (!success)
            {
                return NotFound();
            }

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

    [HttpGet("filter")]
    public async Task<IActionResult> Filter(string status)
    {
        var allProjects = await _projectService.GetAllProjectsAsync();

        var filtered = allProjects;
        if (!string.IsNullOrWhiteSpace(status) && status != "all")
        {
            filtered = allProjects
                .Where(p => p.Status?.StatusName?.Equals(status, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        var viewModels = filtered.Select(project => new ProjectViewModel
        {
            Id = project.Id,
            AvatarUrl = project.AvatarUrl,
            Name = project.ProjectName,
            Company = project.Client?.ClientName ?? "Unknown",
            Description = project.Description,
            Status = project.Status?.StatusName ?? "N/A",
            EndDate = project.EndDate,
            Deadline = DateHelper.FormatDeadline(project.EndDate),
            TeamMembers = project.ProjectMembers.Select(pm => new TeamMember
            {
                Name = pm.Member.FirstName + " " + pm.Member.LastName,
                AvatarUrl = string.IsNullOrWhiteSpace(pm.Member.AvatarUrl) ? "/images/avatar.svg" : pm.Member.AvatarUrl
            }).ToList()
        }).ToList();

        var html = await RenderViewAsync("Views/Shared/Partials/Sections/_ProjectList.cshtml", viewModels, partial: false);

        return Json(new
        {
            html,
            counts = new
            {
                all = allProjects.Count,
                started = allProjects.Count(p => p.Status?.StatusName == "In Progress"),
                completed = allProjects.Count(p => p.Status?.StatusName == "Completed"),
                notStarted = allProjects.Count(p => p.Status?.StatusName == "Not Started")
            }
        });
    }

    private async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = true)
    {
        var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

        using var sw = new StringWriter();
        var viewResult = partial
            ? _viewEngine.FindView(actionContext, viewName, false)
            : _viewEngine.GetView(null, viewName, false);

        if (!viewResult.Success)
            throw new InvalidOperationException($"Could not find view {viewName}");

        var viewDictionary = new ViewDataDictionary<TModel>(
            metadataProvider: new EmptyModelMetadataProvider(),
            modelState: ModelState)
        {
            Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            TempData,
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }

    [HttpGet("addmembermodal/{projectId}")]
    public async Task<IActionResult> AddMemberModal(int projectId)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId);
        if (project == null)
        {
            return NotFound();
        }

        var preselected = project.ProjectMembers.Select(pm => new
        {
            id = pm.MemberId,
            tagName = $"{pm.Member.FirstName} {pm.Member.LastName}",
            avatar = string.IsNullOrWhiteSpace(pm.Member.AvatarUrl) ? "/images/avatar.svg" : pm.Member.AvatarUrl
        }).ToList();

        ViewBag.PreselectedTeamMembersJson = JsonSerializer.Serialize(preselected);

        var model = new AddMemberViewModel
        {
            ProjectId = project.Id,
            ProjectName = project.ProjectName
        };

        return PartialView("~/Views/Shared/Partials/Sections/_AddMemberModal.cshtml", model);
    }


    [HttpPost("addmembers")]
    public async Task<IActionResult> AddMembers([FromForm] Infrastructure.DTOs.AddMembersRequest request)
    {
        if (request.ProjectId <= 0 || request.SelectedTeamMemberIds == null || !request.SelectedTeamMemberIds.Any())
        {
            return BadRequest(new { success = false, message = "Invalid project or members." });
        }

        try
        {
            await _projectService.AddMembersToProjectAsync(request.ProjectId, request.SelectedTeamMemberIds);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding members to project {ProjectId}", request.ProjectId);
            return StatusCode(500, new { success = false, message = "Error adding members." });
        }
    }







}
