using Infrastructure.Entities;
using Infrastructure.Interfaces;
using MainApp.Models;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Hosting;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MainApp.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/clients")]
public class ClientsController : Controller
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientsController> _logger;
    private readonly FileService _fileService;

    public ClientsController(IClientService clientService, ILogger<ClientsController> logger, FileService fileService)
    {
        _clientService = clientService;
        _logger = logger;
        _fileService = fileService;
    }


    [HttpGet("list")]
    public async Task<IActionResult> GetClientsList()
    {
        var clients = await _clientService.GetAllClientsAsync();
        if (clients == null || !clients.Any())
        {
            _logger.LogWarning(clients == null ? "Clients list is NULL!" : "Clients list is EMPTY!");
            clients = new List<ClientEntity>();
        }
        else
        {
            _logger.LogInformation($"Retrieved {clients.Count} clients from the database.");
        }

        return PartialView("Partials/Sections/_ClientTableBody", clients);
    }

    [HttpPost("delete-multiple")]
    public async Task<IActionResult> DeleteMultipleClients([FromBody] List<int> clientIds)
    {
        if (clientIds == null || !clientIds.Any())
        {
            return BadRequest(new { success = false, message = "No clients selected for deletion." });
        }

        try
        {
            int deletedCount = await _clientService.DeleteMultipleClientsAsync(clientIds);
            return Json(new { success = true, deleted = deletedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting multiple clients");
            return StatusCode(500, new { success = false, message = "Error deleting clients" });
        }
    }

    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadClientAvatar(IFormFile file)
    {
        string? fileUrl = await _fileService.SaveFileAsync(file, "clients"); //  Use FileService
        if (fileUrl == null)
        {
            return BadRequest("Error uploading file.");
        }

        return Ok(new { url = fileUrl });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("Partials/Sections/_CreateClient"); // AJAX will now fetch this
    }



    [HttpPost("create")]
    public async Task<IActionResult> CreateClient(ClientCreateFormModel form)
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
            avatarUrl = await _fileService.SaveFileAsync(form.File, "clients");
        }

        var newClient = new ClientEntity
        {
            ClientName = form.ClientName,
            ContactPerson = form.ContactPerson,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "N/A",
            Address = form.Address ?? "Unknown",
            CreatedAt = DateTime.UtcNow,
            AvatarUrl = avatarUrl
        };

        try
        {
            await _clientService.CreateClientAsync(newClient);
            _logger.LogInformation("Client Created Successfully: {@Client}", newClient);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Creating Client: {@Form}", form);
            return StatusCode(500, new { success = false, message = "Internal Server Error" });
        }
    }

    [HttpGet("editclient/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null) return NotFound();

        var model = new ClientEditFormModel
        {
            Id = client.Id,
            ClientName = client.ClientName,
            ContactPerson = client.ContactPerson,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            Address = client.Address,
            AvatarUrl = client.AvatarUrl 
        };

        return PartialView("~/Views/Shared/Partials/Sections/_EditClient.cshtml", model);
    }
    [HttpPost("editclient")]
    public async Task<IActionResult> Edit(ClientEditFormModel form)
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
            var client = await _clientService.GetClientByIdAsync(form.Id);
            if (client == null) return NotFound();

            if (form.File != null)
            {
                var uploadedFilePath = await _fileService.SaveFileAsync(form.File, "clients");
                if (!string.IsNullOrEmpty(uploadedFilePath))
                {
                    client.AvatarUrl = uploadedFilePath;
                }
            }

            client.ClientName = form.ClientName;
            client.ContactPerson = form.ContactPerson;
            client.Email = form.Email;
            client.PhoneNumber = form.PhoneNumber;
            client.Address = form.Address;

            await _clientService.UpdateClientAsync(client);
            _logger.LogInformation("Client Updated Successfully: {@Client}", client);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Updating Client: {@Form}", form);
            return StatusCode(500, new { success = false, message = "Error updating client. Please try again." });
        }
    }

}
