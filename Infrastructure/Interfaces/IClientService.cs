using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IClientService
{
    Task<ClientEntity> CreateClientAsync(ClientEntity newClient); // ✅ Create
    Task<List<ClientEntity>> GetAllClientsAsync(); // ✅ Read (Get All)
    Task<ClientEntity?> GetClientByIdAsync(int id); // ✅ Read (Get One)
    Task<bool> UpdateClientAsync(ClientEntity updatedClient); // ✅ Update
    Task<bool> DeleteClientAsync(int id); // ✅ Delete
}
