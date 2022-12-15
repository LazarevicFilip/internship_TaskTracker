using DataAccess.DAL.Core;
using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IProjectService
    {
        Task<IList<ProjectDto>> GetAll(SearchDto dto);
        Task<ProjectDto> GetOne(int id);
        Task Update(ProjectDto task,int id);
        Task Insert(ProjectDto task);
        Task Delete(int id);
        Task AddTasksToProject(AddTasksDto tasks, int id);
        Task RemoveTasksFromProject(AddTasksDto tasks, int id);
    }
}
