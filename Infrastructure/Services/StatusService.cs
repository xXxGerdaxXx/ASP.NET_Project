using Infrastructure.DTOs;
using Infrastructure.Interfaces;
using Infrastructure.Entities;


public class StatusService(IStatusRepository _statusRepository) : IStatusService
{
    public async Task<IEnumerable<StatusDTO>> GetAllAsync()
    {
        var statuses = await _statusRepository.GetAllAsync();
        return statuses.Select(s => new StatusDTO
        {
            Id = s.Id,
            StatusName = s.StatusName // 👈 also corrected this property
        });
    }

    public async Task<StatusDTO?> GetByIdAsync(int id)
    {
        var status = await _statusRepository.GetByIdAsync(id);
        if (status == null) return null;

        return new StatusDTO
        {
            Id = status.Id,
            StatusName = status.StatusName // 👈 match the property name from entity
        };
    }
}
