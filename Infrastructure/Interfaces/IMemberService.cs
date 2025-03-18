using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IMemberService
{
    Task<List<MemberEntity>> GetAllMembersAsync();
    Task<MemberEntity?> GetMemberByIdAsync(int id);
    Task<MemberEntity?> CreateMemberAsync(MemberEntity member); // Updated return type
    Task<bool> UpdateMemberAsync(MemberEntity member);
    Task<bool> DeleteMemberAsync(int id);
}
