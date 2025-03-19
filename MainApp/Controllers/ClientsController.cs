using Infrastructure.Entities;
using Infrastructure.Interfaces;
using MainApp.Models;
using Microsoft.AspNetCore.Hosting;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MainApp.Controllers;

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

    // ✅ Get all clients (Partial View)
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
    // ✅ Handle Avatar Uploads
    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadClientAvatar(IFormFile file)
    {
        string? fileUrl = await _fileService.SaveFileAsync(file, "clients"); // ✅ Use FileService
        if (fileUrl == null)
        {
            return BadRequest("Error uploading file.");
        }

        return Ok(new { url = fileUrl });
    }
    // GET: /admin/clients/create
    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("Partials/Sections/_CreateClient"); // ✅ AJAX will now fetch this
    }


    // ✅ Create a new client
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
        // ✅ Save uploaded file (Avatar)
        string? avatarUrl = null;
        if (form.File != null)
        {
            avatarUrl = await _fileService.SaveFileAsync(form.File, "clients"); // ✅ Save file
        }
        var newClient = new ClientEntity
        {
            ClientName = form.ClientName,
            ContactPerson = form.ContactPerson,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "N/A",
            Address = form.Address ?? "Unknown",
            CreatedAt = DateTime.UtcNow,
            AvatarUrl = avatarUrl // ✅ Ensure avatar URL is saved
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
            return StatusCode(500, new { success = false, message = ex.Message });
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
            AvatarUrl = client.AvatarUrl // ✅ Ensure avatar is passed
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

            // ✅ Save file only if a new one is uploaded
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
            //client.CreatedAt = DateTime.UtcNow;
            //client.AvatarUrl = form.AvatarUrl;



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



    //[HttpPost("editclient")]
    //public async Task<IActionResult> Edit(ClientEditFormModel form)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        var errors = ModelState
    //            .Where(x => x.Value?.Errors.Count > 0)
    //            .ToDictionary(
    //                kvp => kvp.Key,
    //                kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
    //            );

    //        _logger.LogWarning("Form validation failed: {@Errors}", errors);
    //        return BadRequest(new { success = false, errors });
    //    }

    //    try
    //    {
    //        var updatedClient = new ClientEntity
    //        {
    //            Id = form.Id,
    //            ClientName = form.ClientName,
    //            ContactPerson = form.ContactPerson,
    //            Email = form.Email,
    //            PhoneNumber = form.PhoneNumber,
    //            Address = form.Address,
    //            AvatarUrl = form.AvatarUrl 
    //        };

    //        await _clientService.UpdateClientAsync(updatedClient);
    //        _logger.LogInformation("Client Updated Successfully: {@Client}", updatedClient);

    //        return Json(new { success = true });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error Updating Client: {@Form}", form);
    //        return StatusCode(500, new { success = false, message = "Error updating client. Please try again." });
    //    }
    //}

    //private async Task<string?> SaveFileAsync(IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return null;

    //    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clients");
    //    Directory.CreateDirectory(uploadsPath);

    //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    //    var filePath = Path.Combine(uploadsPath, fileName);

    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await file.CopyToAsync(stream);
    //    }

    //    return $"/images/clients/{fileName}"; // ✅ Returns relative URL
    //}



    //[HttpPost("EditClient")]
    //public async Task<IActionResult> EditClient([FromForm] ClientEditFormModel form)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        var errors = ModelState
    //            .Where(x => x.Value?.Errors.Count > 0)
    //            .ToDictionary(
    //                kvp => kvp.Key,
    //                kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
    //            );

    //        _logger.LogWarning("Form validation failed: {@Errors}", errors);
    //        return BadRequest(new { success = false, errors });
    //    }

    //    var existingClient = await _clientService.GetClientByIdAsync(form.Id);
    //    if (existingClient == null)
    //    {
    //        return NotFound(new { success = false, message = "Client not found." });
    //    }

    //    // ✅ Handle file upload if provided
    //    if (form.File != null && form.File.Length > 0)
    //    {
    //        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(form.File.FileName)}";
    //        var filePath = Path.Combine("wwwroot/uploads", fileName);

    //        using (var stream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await form.File.CopyToAsync(stream);
    //        }

    //        existingClient.AvatarUrl = $"/uploads/{fileName}";
    //    }

    //    // ✅ Update client details
    //    existingClient.ClientName = form.ClientName;
    //    existingClient.ContactPerson = form.ContactPerson;
    //    existingClient.Email = form.Email;
    //    existingClient.PhoneNumber = form.PhoneNumber ?? "N/A";
    //    existingClient.Address = form.Address ?? "Unknown";

    //    try
    //    {
    //        await _clientService.UpdateClientAsync(existingClient);
    //        _logger.LogInformation("Client Updated Successfully: {@Client}", existingClient);

    //        return Json(new { success = true, avatarUrl = existingClient.AvatarUrl });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error Updating Client: {@Form}", form);
    //        return StatusCode(500, new { success = false, message = ex.Message });
    //    }
    //}

    //[HttpGet("get/{id}")]
    //public async Task<IActionResult> GetClientById(int id)
    //{
    //    var client = await _clientService.GetClientByIdAsync(id);
    //    if (client == null)
    //    {
    //        return NotFound(new { success = false, message = "Client not found." });
    //    }

    //    return Json(new { success = true, client });
    //}

}
