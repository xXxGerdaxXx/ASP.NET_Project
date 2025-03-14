using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Business.Services;

public class MemberService(IMemberRepository memberRepository)
{
    private readonly IMemberRepository _memberRepository = memberRepository;

    // ✅ CREATE - Add a new member
    public async Task<MemberEntity> CreateMemberAsync(MemberEntity newMember)
    {
        return await _memberRepository.CreateMemberAsync(newMember);
    }

    // ✅ READ - Get all members
    public async Task<List<MemberEntity>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllMembersAsync();
    }

    // ✅ READ - Get member by ID
    public async Task<MemberEntity?> GetMemberByIdAsync(int id)
    {
        return await _memberRepository.GetMemberByIdAsync(id);
    }

    // ✅ UPDATE - Edit an existing member
    public async Task<bool> UpdateMemberAsync(MemberEntity updatedMember)
    {
        return await _memberRepository.UpdateMemberAsync(updatedMember);
    }

    // ✅ DELETE - Remove a member
    public async Task<bool> DeleteMemberAsync(int id)
    {
        return await _memberRepository.DeleteMemberAsync(id);
    }
}
