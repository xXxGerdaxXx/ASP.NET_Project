using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Business.Services;

public class ClientService(IClientRepository clientRepository)
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
        return await _clientRepository.GetAllClientsAsync();
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
