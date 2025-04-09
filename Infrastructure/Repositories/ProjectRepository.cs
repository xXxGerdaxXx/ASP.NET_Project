using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    public override async Task<List<ProjectEntity>> GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Status)
            .Include(p => p.ProjectMembers)
            .ThenInclude(pm => pm.Member)

            .ToListAsync();
    }

    public override async Task<ProjectEntity?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Where(p => p.Id == id)
            .Include(p => p.Client)
            .Include(p => p.Status)
            .Include(p => p.ProjectMembers)
            .ThenInclude(pm => pm.Member)
            .FirstOrDefaultAsync();


    }

    
}
