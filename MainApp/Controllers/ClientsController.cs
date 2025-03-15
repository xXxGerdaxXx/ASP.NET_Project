using MainApp.Models;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace MainApp.Controllers;

public class ClientsController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    [HttpPost]
    [Route("Clients")]
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

            return BadRequest(new { success = false, errors });
        }

        var newClient = new ClientEntity
        {
            ClientName = form.ClientName,
            ContactPerson = form.ContactPerson,
            Email = form.Email,
            PhoneNumber = form.Phone,
            Address = form.Address
        };

        await _clientService.CreateClientAsync(newClient);

        // ✅ Instead of redirecting, return updated HTML for client table
        var clients = await _clientService.GetAllClientsAsync();
        return PartialView("Partials/Sections/_ClientList", clients);
    }


    [HttpGet]
    public async Task<IActionResult> Clients()
    {
        var clients = await _clientService.GetAllClientsAsync();

        // ✅ Ensure the model is not null before passing it to the view
        return View(clients ?? new List<ClientEntity>());
    }


    [HttpPost]
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

        var updatedClient = new ClientEntity
        {
            Id = form.Id,  // Ensure the ID is set
            ClientName = form.ClientName,
            ContactPerson = form.ContactPerson,
            Email = form.Email,
            PhoneNumber = form.Phone,
            Address = form.Address
        };

        var success = await _clientService.UpdateClientAsync(updatedClient);
        if (!success)
        {
            return NotFound(new { success = false, message = "Client not found." });
        }

        return Ok(new { success = true });
    }
}
//[Route("clients")]
//public class ClientsController : Controller
//{


//    // ✅ READ - Get All Clients (Display Clients List)
//    [HttpGet("")]
//    public async Task<IActionResult> Index()
//    {
//        var clients = await _clientRepository.GetAllClientsAsync();
//        return View(clients);  // Return a view with the list of clients
//    }

//    // ✅ CREATE - Show the Create Client Form
//    [HttpGet("create")]
//    public IActionResult Create()
//    {
//        return PartialView("_Create");  // Render as a partial view
//    }

//    // ✅ CREATE - Handle Client Form Submission
//    [HttpPost("create")]
//    public async Task<IActionResult> Create(ClientCreateFormModel model)
//    {
//        if (!ModelState.IsValid)
//        {
//            return PartialView("_Create", model);  // Return Partial if validation fails
//        }

//        // Convert ClientCreateFormModel to ClientEntity
//        var newClient = new ClientEntity
//        {
//            ClientName = model.ClientName,
//            ContactPerson = model.ContactPerson,
//            Email = model.Email,
//            PhoneNumber = model.Phone
//        };

//        // Save the client to the database
//        await _clientRepository.CreateClientAsync(newClient);

//        return RedirectToAction("Index");  // Redirect to the client list
//    }

//    // ✅ READ - Get Client by ID (For editing or viewing details)
//    [HttpGet("edit/{id}")]
//    public async Task<IActionResult> Edit(int id)
//    {
//        var client = await _clientRepository.GetClientByIdAsync(id);
//        if (client == null)
//        {
//            return NotFound();  // Return 404 if client not found
//        }

//        // Convert ClientEntity to ClientCreateFormModel for editing
//        var model = new ClientCreateFormModel
//        {
//            ClientName = client.ClientName,
//            ContactPerson = client.ContactPerson,
//            Email = client.Email,
//            Phone = client.PhoneNumber
//        };

//        return PartialView("_Edit", model);  // Render Edit form as a partial view
//    }

//    // ✅ UPDATE - Handle the Edit Client Form Submission
//    [HttpPost("edit")]
//    public async Task<IActionResult> Edit(ClientCreateFormModel model)
//    {
//        if (!ModelState.IsValid)
//        {
//            return PartialView("_Edit", model);  // Return Partial if validation fails
//        }

//        // Convert ClientCreateFormModel to ClientEntity
//        var updatedClient = new ClientEntity
//        {
//            Id = model.Id,  // Ensure the Id is passed and mapped
//            ClientName = model.ClientName,
//            ContactPerson = model.ContactPerson,
//            Email = model.Email,
//            PhoneNumber = model.Phone
//        };

//        // Update client in the database
//        var success = await _clientRepository.UpdateClientAsync(updatedClient);

//        if (!success)
//        {
//            ModelState.AddModelError("", "Failed to update client.");
//            return PartialView("_Edit", model);  // Return Partial with error message
//        }

//        return RedirectToAction("Index");  // Redirect to client list after successful update
//    }

//    // ✅ DELETE - Delete Client by ID
//    [HttpPost("delete/{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        var success = await _clientRepository.DeleteClientAsync(id);

//        if (!success)
//        {
//            ModelState.AddModelError("", "Failed to delete client.");
//        }

//        return RedirectToAction("Index");  // Redirect back to client list
//    }
//}
