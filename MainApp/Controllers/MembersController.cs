using Infrastructure.Entities;
using Infrastructure.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace MainApp.Controllers;

[Route("admin/members")]
public class MembersController(IMemberService memberService, ILogger<MembersController> logger, FileService fileService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    private readonly ILogger<MembersController> _logger = logger;
    private readonly FileService _fileService = fileService;

    [HttpGet("list")]
    public async Task<IActionResult> GetMembersList()
    {
        var members = await _memberService.GetAllMembersAsync();

        if (members == null || !members.Any())
        {
            _logger.LogWarning(members == null ? "Members list is NULL!" : "Members list is EMPTY!");
            members = new List<MemberEntity>();
        }
        else
        {
            _logger.LogInformation($"Retrieved {members.Count} members from the database.");
        }

        return PartialView("Partials/Sections/_MemberTableBody", members); 
    }

    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadClientAvatar(IFormFile file)
    {
        string? fileUrl = await _fileService.SaveFileAsync(file, "members"); 
        if (fileUrl == null)
        {
            return BadRequest("Error uploading file.");
        }

        return Ok(new { url = fileUrl });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("Partials/Sections/_CreateMember"); // Now used for AJAX
    }


    // POST: /members/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateMember(MemberCreateForm form)
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
        if (form.File != null)
        {
            avatarUrl = await _fileService.SaveFileAsync(form.File, "members");
        }
        var newMember = new MemberEntity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "N/A",
            Address = form.Address ?? "Unknown",
            DateOfBirth = form.DateOfBirth,
            JobTitle = form.JobTitle,
            AvatarUrl = avatarUrl
        };


        try
        {
            await _memberService.CreateMemberAsync(newMember);
            _logger.LogInformation("Member Created Successfully: {@Member}", newMember);

            return Json(new { success = true }); // AJAX-friendly response
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Creating Member: {@Form}", form);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null) return NotFound();

        var model = new MemberEditFormModel
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            Address = member.Address,
            DateOfBirth = member.DateOfBirth,
            JobTitle = member.JobTitle,
            AvatarUrl = member.AvatarUrl
        };

        return PartialView("~/Views/Shared/Partials/Sections/_EditMember.cshtml", model);
    }


    [HttpPost("editmember")]
    public async Task<IActionResult> Edit(MemberEditFormModel form)
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
            var member = await _memberService.GetMemberByIdAsync(form.Id);
            if (member == null) return NotFound();

            // Save file only if a new one is uploaded
            if (form.File != null)
            {
                var uploadedFilePath = await _fileService.SaveFileAsync(form.File, "members");
                if (!string.IsNullOrEmpty(uploadedFilePath))
                {
                    member.AvatarUrl = uploadedFilePath;
                }
            }

            // Update member properties
            member.FirstName = form.FirstName;
            member.LastName = form.LastName;
            member.Email = form.Email;
            member.PhoneNumber = form.PhoneNumber;
            member.Address = form.Address;
            member.JobTitle = form.JobTitle;

            await _memberService.UpdateMemberAsync(member);
            _logger.LogInformation("Member Updated Successfully: {@Member}", member);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Updating Member: {@Form}", form);
            return StatusCode(500, new { success = false, message = "Error updating member. Please try again." });
        }
    }

    [HttpPost("delete/{memberId}")]
    public async Task<IActionResult> DeleteMember(int memberId)
    {
        if (memberId <= 0)
        {
            return BadRequest(new { success = false, message = "Invalid member ID." });
        }

        try
        {
            bool isDeleted = await _memberService.DeleteMemberAsync(memberId);

            if (!isDeleted)
            {
                return NotFound(new { success = false, message = "Member not found or could not be deleted." });
            }

            return Json(new { success = true, message = "Member deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting member ID {MemberId}", memberId);
            return StatusCode(500, new { success = false, message = "Error deleting the member." });
        }
    }


}
