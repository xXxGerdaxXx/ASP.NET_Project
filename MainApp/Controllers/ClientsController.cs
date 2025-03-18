using Infrastructure.Entities;
using Infrastructure.Interfaces;
using MainApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace MainApp.Controllers;

[Route("admin/clients")]
public class ClientsController : Controller
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    // ✅ THIS RETURNS THE PARTIAL VIEW (NOT A FULL PAGE)
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

        return PartialView("Partials/Sections/_ClientTableBody", clients); // ✅ Returns only the partial!
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

            _logger.LogWarning("⚠️ Form validation failed: {@Errors}", errors);
            return BadRequest(new { success = false, errors });
        }

        var newClient = new ClientEntity
        {
            ClientName = form.ClientName,
            ContactPerson = form.ContactPerson,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber ?? "N/A",
            Address = form.Address ?? "Unknown",
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _clientService.CreateClientAsync(newClient);
            _logger.LogInformation("✅ Client Created Successfully: {@Client}", newClient);

            return Json(new { success = true }); // ✅ AJAX-friendly response
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error Creating Client: {@Form}", form);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
