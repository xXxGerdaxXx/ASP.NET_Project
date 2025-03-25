using Infrastructure.Interfaces;
using Infrastructure.Entities;

namespace Infrastructure.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientEntity?> CreateClientAsync(ClientEntity newClient)
    {
        if (newClient == null)
        {
            Console.WriteLine("Attempted to create a null client.");
            return null;
        }

        var createdClient = await _clientRepository.CreateAsync(newClient);
        return createdClient ?? null;
    }

    public async Task<List<ClientEntity>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync() ?? new List<ClientEntity>();
        Console.WriteLine($"ClientService retrieved {clients.Count} clients.");
        return clients;
    }

    public async Task<ClientEntity?> GetClientByIdAsync(int id)
    {
        if (id <= 0)
        {
            Console.WriteLine("Invalid client ID.");
            return null;
        }

        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
        {
            Console.WriteLine($"Client with ID {id} not found.");
        }

        return client;
    }

    public async Task<ClientEntity?> UpdateClientAsync(ClientEntity updatedClient)
    {
        if (updatedClient == null || updatedClient.Id <= 0)
        {
            Console.WriteLine("Invalid client update request.");
            return null;
        }

        var existingClient = await _clientRepository.GetByIdAsync(updatedClient.Id);
        if (existingClient == null)
        {
            Console.WriteLine($"Cannot update. Client with ID {updatedClient.Id} not found.");
            return null;
        }

        var success = await _clientRepository.UpdateAsync(updatedClient);
        return success ? updatedClient : null;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        if (id <= 0)
        {
            Console.WriteLine("Invalid client ID for deletion.");
            return false;
        }

        var existingClient = await _clientRepository.GetByIdAsync(id);
        if (existingClient == null)
        {
            Console.WriteLine($"Cannot delete. Client with ID {id} not found.");
            return false;
        }

        return await _clientRepository.DeleteAsync(id);
    }

    public async Task<int> DeleteMultipleClientsAsync(List<int> clientIds)
    {
        if (clientIds == null || !clientIds.Any())
        {
            Console.WriteLine("No clients provided for deletion.");
            return 0;
        }

        var deletedCount = await _clientRepository.DeleteMultipleClientsAsync(clientIds);
        Console.WriteLine($"Deleted {deletedCount} clients.");
        return deletedCount;
    }
}
