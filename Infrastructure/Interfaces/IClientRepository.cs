using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IClientRepository : IBaseRepository<ClientEntity>
{
    Task<int> DeleteMultipleClientsAsync(List<int> clientIds);
}
