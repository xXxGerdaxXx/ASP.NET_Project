using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));


    // ✅ CREATE (Add New Client)
    public async Task<ClientEntity> CreateClientAsync(ClientEntity newClient)
    {
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync();
        return newClient;
    }

    public async Task<List<ClientEntity>> GetAllClientsAsync()
    {
        var clients = await _context.Clients.ToListAsync();

        Console.WriteLine($"📢 Repository retrieved {clients.Count} clients.");

        if (clients.Count == 0)
        {
            Console.WriteLine("⚠️ No clients found in the database!");
        }

        return clients;
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

    public async Task<int> DeleteMultipleClientsAsync(List<int> clientIds)
    {
        var clientsToDelete = await _context.Clients
            .Where(client => clientIds.Contains(client.Id))
            .ToListAsync();

        if (!clientsToDelete.Any())
            return 0;

        _context.Clients.RemoveRange(clientsToDelete);
        await _context.SaveChangesAsync();

        return clientsToDelete.Count;
    }

}
