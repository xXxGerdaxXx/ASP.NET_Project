using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<List<MemberEntity>> GetAllMembersAsync()
        {
            return await _memberRepository.GetAllMembersAsync();
        }

        public async Task<MemberEntity?> GetMemberByIdAsync(int id)
        {
            return await _memberRepository.GetMemberByIdAsync(id);
        }

        public async Task<MemberEntity?> CreateMemberAsync(MemberEntity newMember)
        {
            if (newMember == null)
            {
                Console.WriteLine("Attempted to create a null member.");
                return null;
            }

            var createdMember = await _memberRepository.CreateMemberAsync(newMember);
            return createdMember ?? null;
        }

        public async Task<bool> UpdateMemberAsync(MemberEntity member)
        {
            return await _memberRepository.UpdateMemberAsync(member);
        }

        public async Task<bool> DeleteMemberAsync(int memberId)
        {
            if (memberId <= 0)
            {
                Console.WriteLine("Invalid member ID for deletion.");
                return false;
            }

            var existingMember = await _memberRepository.GetMemberByIdAsync(memberId);
            if (existingMember == null)
            {
                Console.WriteLine($"Cannot delete. Member with ID {memberId} not found.");
                return false;
            }

            return await _memberRepository.DeleteMemberAsync(memberId);
        }

    }
}
