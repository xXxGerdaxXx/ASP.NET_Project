using System.Threading.Tasks;
using Infrastructure.Entities;

namespace Infrastructure.Interfaces;

public interface IProjectService 
{
    Task<List<ProjectEntity>> GetAllProjectsAsync();
    Task<ProjectEntity?> GetProjectByIdAsync(int id);
    Task<ProjectEntity> CreateProjectAsync(ProjectEntity project);
    Task<bool> UpdateProjectAsync(ProjectEntity project);
    Task<bool> DeleteProjectAsync(int id);
}
