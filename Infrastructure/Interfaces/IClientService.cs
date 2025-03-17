using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IClientService
{
    Task<ClientEntity?> CreateClientAsync(ClientEntity newClient); 
    Task<List<ClientEntity>> GetAllClientsAsync();
    Task<ClientEntity?> GetClientByIdAsync(int id); 
    Task<ClientEntity?> UpdateClientAsync(ClientEntity updatedClient);
    Task<bool> DeleteClientAsync(int id);
}
