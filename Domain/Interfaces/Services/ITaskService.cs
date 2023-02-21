using DataAccess.DAL.Core;
using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IList<TaskDto>> GetAll(PagingDto dto);
        Task<TaskDto> GetOne(int id);
        Task Update(TaskDto task, int id);
        Task Insert(TaskDto task);
        Task Delete(int id);

    }
}
