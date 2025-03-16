using Infrastructure.Interfaces;
using Infrastructure.Entities;

namespace Infrastructure.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    // ✅ CREATE (Add New Client)
    public async Task<ClientEntity> CreateClientAsync(ClientEntity newClient)
    {
        return await _clientRepository.CreateClientAsync(newClient);
    }

    // ✅ READ (Get All Clients)
    public async Task<List<ClientEntity>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllClientsAsync();
        Console.WriteLine($"🔎 ClientService retrieved {clients.Count} clients.");
        return clients;
    }

    // ✅ READ (Get Client By ID)
    public async Task<ClientEntity?> GetClientByIdAsync(int id)
    {
        return await _clientRepository.GetClientByIdAsync(id);
    }

    // ✅ UPDATE (Edit Client Details)
    public async Task<bool> UpdateClientAsync(ClientEntity updatedClient)
    {
        return await _clientRepository.UpdateClientAsync(updatedClient);
    }

    // ✅ DELETE (Remove Client)
    public async Task<bool> DeleteClientAsync(int id)
    {
        return await _clientRepository.DeleteClientAsync(id);
    }
}
