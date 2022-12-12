using BusinessLogic.BAL.Exceptions;
using DataAccess.DAL.Core;
using Domain.Dto;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TaskModel>> GetAll()
        {
            return await _unitOfWork.Repository<TaskModel>().GetAllAsync();
        }

        public async Task<TaskModel> GetOne(int id)
        {
           return await _unitOfWork.Repository<TaskModel>().FindAsync(id);
        }

        public async Task Insert(TaskDto task)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                var t = new TaskModel()
                {
                    Name = task.Name,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    Project = await _unitOfWork.Repository<ProjectModel>().FindAsync(task.Project)
                };
                await _unitOfWork.Repository<TaskModel>().InsertAsync(t, true);
                await _unitOfWork.CommitTransaction();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error occurred at {repo} insert method",typeof(TaskService));
                await _unitOfWork.RollbackTransaction();
            }
           
        }

        public async Task Update(TaskDto task,int id)
        {
            try
            {
                task.Id = id;
                await _unitOfWork.BeginTransaction();
                
                var row = await _unitOfWork.Repository<TaskModel>().FindAsync(task.Id);
                if(row == null)
                {
                    throw new EntityNotFoundException(nameof(TaskDto),id);
                }
                row.Name= task.Name;
                row.Status= task.Status;
                row.Priority= task.Priority;
                row.Description= task.Description;
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error occurred at {repo} update method", typeof(TaskService));
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }
    }
}
