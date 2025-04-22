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
        DateTime? dob = null;
        if (form.File != null)
        {
            avatarUrl = await _fileService.SaveFileAsync(form.File, "members");
        }
        if (form.BirthYear.HasValue && form.BirthMonth.HasValue && form.BirthDay.HasValue)
        {
            dob = new DateTime(form.BirthYear.Value, form.BirthMonth.Value, form.BirthDay.Value);
        }
        var newMember = new MemberEntity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "N/A",
            Address = form.Address ?? "Unknown",
            DateOfBirth = dob ?? default,
            JobTitle = form.JobTitle,
            AvatarUrl = avatarUrl
        };


        try
        {
            await _memberService.CreateMemberAsync(newMember);
            _logger.LogInformation("Member Created Successfully: {@Member}", newMember);

            return Json(new { success = true });
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
            JobTitle = member.JobTitle ?? 0,
            AvatarUrl = member.AvatarUrl,
            BirthDay = member.DateOfBirth.Day,
            BirthMonth = member.DateOfBirth.Month,
            BirthYear = member.DateOfBirth.Year
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

            member.FirstName = form.FirstName;
            member.LastName = form.LastName;
            member.Email = form.Email;
            member.PhoneNumber = form.PhoneNumber;
            member.Address = form.Address;
            member.JobTitle = form.JobTitle;
            member.DateOfBirth = new DateTime(form.BirthYear, form.BirthMonth, form.BirthDay);
            try
            {
                var dob = new DateTime(form.BirthYear, form.BirthMonth, form.BirthDay);
            }
            catch
            {
                ModelState.AddModelError("BirthDay", "Invalid date of birth selected.");
                return BadRequest(ModelState);
            }

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

    [HttpGet("search")]
    public async Task<JsonResult> SearchMembers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var members = await _memberService.GetAllMembersAsync();

        var filtered = members
            .Where(m => m.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        m.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(m => new
            {
                id = m.Id,
                tagName = $"{m.FirstName} {m.LastName}",
                avatar = string.IsNullOrWhiteSpace(m.AvatarUrl) ? "/images/avatar.svg" : m.AvatarUrl
            })
            .ToList();

        return Json(filtered);

    }


}
