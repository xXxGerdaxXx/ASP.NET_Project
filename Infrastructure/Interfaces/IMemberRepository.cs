using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IMemberRepository
{
    Task<MemberEntity> CreateMemberAsync(MemberEntity newMember); // ✅ Create
    Task<List<MemberEntity>> GetAllMembersAsync(); // ✅ Read (Get All)
    Task<MemberEntity?> GetMemberByIdAsync(int id); // ✅ Read (Get One)
    Task<bool> UpdateMemberAsync(MemberEntity updatedMember); // ✅ Update
    Task<bool> DeleteMemberAsync(int id); // ✅ Delete
}
