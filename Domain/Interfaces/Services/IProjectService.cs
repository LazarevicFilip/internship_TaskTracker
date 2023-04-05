using DataAccess.DAL.Core;
using Domain.Dto;
using Domain.Dto.V1.Request;
using Domain.Dto.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IProjectService
    {
        Task <PagedResponse<ProjectResponseDto>> GetAllAsync(SearchDto dto);
        Task<ProjectResponseDto> GetOneAsync(int id);
        Task UpdateAsync(UpdateProjectRequestDto task,int id);
        Task<ProjectResponseDto> InsertAsync(ProjectRequestDto task);
        Task DeleteAsync(int id);
        Task ForceDeleteAsync(int id);
        Task AddTasksToProjectAsync(AddTasksDto tasks, int id);
        Task RemoveTasksFromProjectAsync(AddTasksDto tasks, int id);
    }
}
