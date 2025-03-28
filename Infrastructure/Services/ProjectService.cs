using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<ProjectEntity>> GetAllProjectsAsync()
        {

            return await _projectRepository.GetAllAsync();
        }

        public async Task<ProjectEntity?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task<ProjectEntity?> CreateProjectAsync(ProjectEntity newProject)
        {
            if (newProject == null)
            {
                Console.WriteLine("Attempted to create a null project.");
                return null;
            }

            var createdProject = await _projectRepository.CreateAsync(newProject);
            return createdProject ?? null;
        }

        public async Task<bool> UpdateProjectAsync(ProjectEntity project)
        {
            return await _projectRepository.UpdateAsync(project);
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            if (projectId <= 0)
            {
                Console.WriteLine("Invalid project ID for deletion.");
                return false;
            }

            var existingProject = await _projectRepository.GetByIdAsync(projectId);
            if (existingProject == null)
            {
                Console.WriteLine($"Cannot delete. roject with ID {projectId} not found.");
                return false;
            }

            return await _projectRepository.DeleteAsync(projectId);
        }
    }
}
