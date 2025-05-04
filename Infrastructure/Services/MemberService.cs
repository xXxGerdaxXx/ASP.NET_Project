using Infrastructure.Entities;
using Infrastructure.Interfaces;


namespace Infrastructure.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        /* Added this for pagination */
        public async Task<IEnumerable<MemberEntity>> GetMembersAsync(int page = 1, int pageSize = 6)
        {
            return await _memberRepository.GetPagedAsync(page, pageSize);
        }
        public async Task<int> GetMembersCountAsync()
        {
            return await _memberRepository.GetCountAsync();
        }

        public async Task<List<MemberEntity>> GetAllMembersAsync()
        {
            return await _memberRepository.GetAllAsync();
        }

        public async Task<MemberEntity?> GetMemberByIdAsync(int id)
        {
            return await _memberRepository.GetByIdAsync(id);
        }

        public async Task<MemberEntity?> CreateMemberAsync(MemberEntity newMember)
        {
            if (newMember == null)
            {
                Console.WriteLine("Attempted to create a null member.");
                return null;
            }

            var createdMember = await _memberRepository.CreateAsync(newMember);
            return createdMember ?? null;
        }

        public async Task<bool> UpdateMemberAsync(MemberEntity member)
        {
            return await _memberRepository.UpdateAsync(member);
        }

        public async Task<bool> DeleteMemberAsync(int memberId)
        {
            if (memberId <= 0)
            {
                Console.WriteLine("Invalid member ID for deletion.");
                return false;
            }

            var existingMember = await _memberRepository.GetByIdAsync(memberId);
            if (existingMember == null)
            {
                Console.WriteLine($"Cannot delete. Member with ID {memberId} not found.");
                return false;
            }

            return await _memberRepository.DeleteAsync(memberId);
        }
    }
}
