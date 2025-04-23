using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IMemberRepository : IBaseRepository<MemberEntity>
{
    /* Added this for pagination */
    Task<IEnumerable<MemberEntity>> GetPagedAsync(int page, int pageSize);
    Task<int> GetCountAsync();
}
