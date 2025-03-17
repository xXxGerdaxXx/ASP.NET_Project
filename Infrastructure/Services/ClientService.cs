using Infrastructure.Interfaces;
using Infrastructure.Entities;

namespace Infrastructure.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    // Create (Returns Created Client or Null)
    public async Task<ClientEntity?> CreateClientAsync(ClientEntity newClient)
    {
        if (newClient == null)
        {
            Console.WriteLine("Attempted to create a null client.");
            return null;
        }

        var createdClient = await _clientRepository.CreateClientAsync(newClient);
        return createdClient ?? null;
    }

    // Read (Get All Clients, Always Returns a List)
    public async Task<List<ClientEntity>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllClientsAsync() ?? new List<ClientEntity>();
        Console.WriteLine($"ClientService retrieved {clients.Count} clients.");
        return clients;
    }

    // Read (Get Client By ID, Returns Null if Not Found)
    public async Task<ClientEntity?> GetClientByIdAsync(int id)
    {
        if (id <= 0)
        {
            Console.WriteLine("Invalid client ID.");
            return null;
        }

        var client = await _clientRepository.GetClientByIdAsync(id);
        if (client == null)
        {
            Console.WriteLine($"Client with ID {id} not found.");
        }

        return client;
    }

    // Update (Returns Updated Client or Null)
    public async Task<ClientEntity?> UpdateClientAsync(ClientEntity updatedClient)
    {
        if (updatedClient == null || updatedClient.Id <= 0)
        {
            Console.WriteLine("Invalid client update request.");
            return null;
        }

        var existingClient = await _clientRepository.GetClientByIdAsync(updatedClient.Id);
        if (existingClient == null)
        {
            Console.WriteLine($"Cannot update. Client with ID {updatedClient.Id} not found.");
            return null;
        }

        var success = await _clientRepository.UpdateClientAsync(updatedClient);
        return success ? updatedClient : null;
    }

    // Delete (Returns True if Deleted, False if Not Found)
    public async Task<bool> DeleteClientAsync(int id)
    {
        if (id <= 0)
        {
            Console.WriteLine("Invalid client ID for deletion.");
            return false;
        }

        var existingClient = await _clientRepository.GetClientByIdAsync(id);
        if (existingClient == null)
        {
            Console.WriteLine($"Cannot delete. Client with ID {id} not found.");
            return false;
        }

        return await _clientRepository.DeleteClientAsync(id);
    }
}
