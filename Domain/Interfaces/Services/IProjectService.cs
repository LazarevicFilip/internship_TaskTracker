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
        Task <PagedResponse<ProjectResponseDto>> GetAll(SearchDto dto);
        Task<ProjectResponseDto> GetOne(int id);
        Task Update(UpdateProjectRequestDto task,int id);
        Task<ProjectResponseDto> Insert(ProjectRequestDto task);
        Task Delete(int id);
        Task forceDelete(int id);
        Task AddTasksToProject(AddTasksDto tasks, int id);
        Task RemoveTasksFromProject(AddTasksDto tasks, int id);
    }
}
