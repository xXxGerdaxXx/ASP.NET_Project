using System.Threading.Tasks;
using Infrastructure.Entities;
using Infrastructure.DTOs;

namespace Infrastructure.Interfaces;

public interface IProjectService 
{
    Task<List<ProjectEntity>> GetAllProjectsAsync();
    Task<ProjectEntity?> GetProjectByIdAsync(int id);
    Task<ProjectEntity?> CreateProjectAsync(ProjectDTO dto);
    Task<bool> UpdateProjectAsync(ProjectUpdateDTO dto);
    Task<bool> DeleteProjectAsync(int id);
    Task<bool> AddMembersToProjectAsync(int projectId, List<int> memberIds);
}
