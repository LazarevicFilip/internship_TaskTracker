using DataAccess.DAL.Core;
using Domain.Dto;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProjectModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectModel> GetOne(int id)
        {
            return await _unitOfWork.Repository<ProjectModel>().FindAsync(id);
        }

        public Task Insert(TaskDto task)
        {
            throw new NotImplementedException();
        }

        public Task Update(TaskDto task)
        {
            throw new NotImplementedException();
        }
    }
}
