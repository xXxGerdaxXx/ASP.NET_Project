using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StatusRepository(AppDbContext context) : BaseRepository<StatusEntity>(context), IStatusRepository
{
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Statuses.AnyAsync(s => s.Id == id);
    }
}
