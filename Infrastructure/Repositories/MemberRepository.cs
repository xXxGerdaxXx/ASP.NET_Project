using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    private readonly AppDbContext _context = context;

    // ✅ CREATE - Add New Member
    public async Task<MemberEntity> CreateMemberAsync(MemberEntity newMember)
    {
        _context.Members.Add(newMember);
        await _context.SaveChangesAsync();
        return newMember;
    }

    // ✅ READ - Get All Members
    public async Task<List<MemberEntity>> GetAllMembersAsync()
    {
        return await _context.Members.ToListAsync();
    }

    // ✅ READ - Get Member By ID
    public async Task<MemberEntity?> GetMemberByIdAsync(int id)
    {
        return await _context.Members.FindAsync(id);
    }

    // ✅ UPDATE - Edit Member
    public async Task<bool> UpdateMemberAsync(MemberEntity updatedMember)
    {
        var existingMember = await _context.Members.FindAsync(updatedMember.Id);
        if (existingMember == null) return false;

        _context.Entry(existingMember).CurrentValues.SetValues(updatedMember);
        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ DELETE - Remove Member
    public async Task<bool> DeleteMemberAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null) return false;

        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }
}
