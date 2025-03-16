using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infrastructure.Interfaces;
using Infrastructure.Entities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MainApp.Models;

[Route("clients")]
public class ClientsController : Controller
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    // ✅ CREATE CLIENT
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

            _logger.LogWarning("⚠️ Form validation failed: {@Errors}", errors);
            return BadRequest(new { success = false, errors });
        }

        // ✅ Handle File Upload
        string? avatarUrl = null;
        if (form.File != null && form.File.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clients");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(form.File.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await form.File.CopyToAsync(fileStream);
            }

            avatarUrl = $"/images/clients/{uniqueFileName}";
        }

        var newClient = new ClientEntity
        {
            ClientName = form.ClientName,
            ContactPerson = form.ContactPerson,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "N/A",
            Address = form.Address ?? "Unknown",
            AvatarUrl = avatarUrl,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _clientService.CreateClientAsync(newClient);
            _logger.LogInformation("✅ Client Created Successfully: {@Client}", newClient);

            return Json(new { success = true }); // ✅ Return JSON for AJAX support
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error Creating Client with Data: {@Form}", form);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    // ✅ FETCH CLIENT LIST
    [HttpGet]
    public async Task<IActionResult> Clients()
    {
        var clients = await _clientService.GetAllClientsAsync();

        if (clients == null || !clients.Any())
        {
            _logger.LogWarning(clients == null ? "❌ Clients list is NULL!" : "⚠️ Clients list is EMPTY!");
            clients = new List<ClientEntity>();
        }
        else
        {
            _logger.LogInformation($"✅ Retrieved {clients.Count} clients from the database.");
        }

        return PartialView("Partials/Sections/_ClientTableBody", clients); // ✅ Fixed incorrect escaping
    }

    private async Task<string?> SaveAvatarAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clients");
        Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

        string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/clients/{uniqueFileName}"; // Return relative URL for display
    }


    // ✅ EDIT CLIENT
    [HttpPost("edit")]
    public async Task<IActionResult> EditClient(ClientEditFormModel form)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        var existingClient = await _clientService.GetClientByIdAsync(form.Id);
        if (existingClient == null)
        {
            _logger.LogWarning("⚠️ Attempted to edit a non-existent client with ID {Id}", form.Id);
            return NotFound(new { success = false, message = "Client not found." });
        }

        existingClient.ClientName = form.ClientName;
        existingClient.ContactPerson = form.ContactPerson;
        existingClient.Email = form.Email;
        existingClient.PhoneNumber = form.PhoneNumber ?? existingClient.PhoneNumber;
        existingClient.Address = form.Address ?? existingClient.Address;

        // ✅ Handle Avatar Update
        if (form.File != null && form.File.Length > 0)
        {
            existingClient.AvatarUrl = await SaveAvatarAsync(form.File);
        }

        var success = await _clientService.UpdateClientAsync(existingClient);
        if (!success)
        {
            _logger.LogError("❌ Client update failed for ID {Id}", form.Id);
            return StatusCode(500, new { success = false, message = "Client update failed." });
        }

        _logger.LogInformation("✅ Client Updated Successfully: {@Client}", existingClient);
        return Ok(new { success = true });
    }

}
