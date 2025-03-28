using Infrastructure.DTOs;

namespace Infrastructure.Interfaces;

public interface IStatusService
{
    Task<IEnumerable<StatusDTO>> GetAllAsync();
    Task<StatusDTO?> GetByIdAsync(int id);
}


