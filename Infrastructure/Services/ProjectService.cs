using Infrastructure.DTOs;
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

        public async Task<ProjectEntity?> CreateProjectAsync(ProjectDTO dto)
        {
            if (dto == null)
            {
                Console.WriteLine("Attempted to create a null project.");
                return null;
            }

            var newProject = new ProjectEntity
            {
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Budget = dto.Budget,
                ClientId = dto.ClientId,
                StatusId = dto.StatusId,
                AvatarUrl = dto.AvatarUrl,
                CreatedByUserId = dto.CreatedByUserId
            };

            // Step 1: Create the project
            var createdProject = await _projectRepository.CreateAsync(newProject);
            if (createdProject == null)
                return null;

            // Step 2: Save team members to ProjectMemberEntity table
            if (dto.ProjectMemberIds != null && dto.ProjectMemberIds.Any())
            {
                createdProject.ProjectMembers = dto.ProjectMemberIds.Select(memberId => new ProjectMemberEntity
                {
                    ProjectId = createdProject.Id,
                    MemberId = memberId
                }).ToList();

                // Update the project with the associated members
                await _projectRepository.UpdateAsync(createdProject);
            }

            return createdProject;
        }

        public async Task<bool> UpdateProjectAsync(ProjectUpdateDTO dto)
        {
            var project = await _projectRepository.GetByIdAsync(dto.Id);
            if (project == null)
            {
                Console.WriteLine("Project not found.");
                return false;
            }

            // Update project properties
            project.ProjectName = dto.ProjectName;
            project.Description = dto.Description;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.Budget = dto.Budget;
            project.ClientId = dto.ClientId;
            project.StatusId = dto.StatusId;
            project.AvatarUrl = dto.AvatarUrl;

            // Update team members:
            project.ProjectMembers.Clear();
            if (dto.TeamMemberIds != null && dto.TeamMemberIds.Any())
            {
                project.ProjectMembers = dto.TeamMemberIds.Select(memberId => new ProjectMemberEntity
                {
                    ProjectId = project.Id,
                    MemberId = memberId
                }).ToList();
            }

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
