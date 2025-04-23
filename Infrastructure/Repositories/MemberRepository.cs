using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories;

public class MemberRepository(AppDbContext context) : BaseRepository<MemberEntity>(context), IMemberRepository
{
    /* Added this for pagination */
    public async Task<IEnumerable<MemberEntity>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Members
            .OrderBy(m => m.Id) 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<int> GetCountAsync()
    {
        return await _context.Members.CountAsync();
    }

}
