using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    private readonly AppDbContext _context = context;

    // ✅ CREATE (Add New Client)
    public async Task<ClientEntity> CreateClientAsync(ClientEntity newClient)
    {
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync();
        return newClient;
    }

    // ✅ READ (Get All Clients)
    public async Task<List<ClientEntity>> GetAllClientsAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    // ✅ READ (Get Client By ID)
    public async Task<ClientEntity?> GetClientByIdAsync(int id)
    {
        return await _context.Clients.FindAsync(id);
    }

    // ✅ UPDATE (Edit Client Details)
    public async Task<bool> UpdateClientAsync(ClientEntity updatedClient)
    {
        var existingClient = await _context.Clients.FindAsync(updatedClient.Id);
        if (existingClient == null) return false;

        _context.Entry(existingClient).CurrentValues.SetValues(updatedClient);
        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ DELETE (Remove Client)
    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}
