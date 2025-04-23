using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IMemberService
{
    /* Added this for pagination */
    Task<IEnumerable<MemberEntity>> GetMembersAsync(int page = 1, int pageSize = 6);
    Task<int> GetMembersCountAsync();


    Task<List<MemberEntity>> GetAllMembersAsync();
    Task<MemberEntity?> GetMemberByIdAsync(int id);
    Task<MemberEntity?> CreateMemberAsync(MemberEntity member); 
    Task<bool> UpdateMemberAsync(MemberEntity member);
    Task<bool> DeleteMemberAsync(int id);
}
