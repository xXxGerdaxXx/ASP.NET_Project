using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class MemberRepository(AppDbContext context) : BaseRepository<MemberEntity>(context), IMemberRepository
{
}
