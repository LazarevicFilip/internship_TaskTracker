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
        Task<IList<ProjectModel>> GetAll();
        Task<ProjectModel> GetOne(int id);
        Task Update(TaskDto task);
        Task Insert(TaskDto task);
        Task Delete(int id);
    }
}
