using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClientRepository(AppDbContext context) : BaseRepository<ClientEntity>(context), IClientRepository
{
    public async Task<int> DeleteMultipleClientsAsync(List<int> clientIds)
    {
        var clientsToDelete = await _dbSet
            .Where(client => clientIds.Contains(client.Id))
            .ToListAsync();

        if (!clientsToDelete.Any())
            return 0;

        _dbSet.RemoveRange(clientsToDelete);
        await _context.SaveChangesAsync();

        return clientsToDelete.Count;
    }
}
