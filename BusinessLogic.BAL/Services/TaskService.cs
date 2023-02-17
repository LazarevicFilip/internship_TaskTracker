﻿using BusinessLogic.BAL.Cache;
using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Logging;
using BusinessLogic.BAL.Validators.TaskValidators;
using DataAccess.DAL.Core;
using DataAccess.DAL.Extensions;
using Domain.Dto;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly ILoggingService _logger;
        private readonly CreateTaskValidator _validator;
        private readonly UpdateTaskValidator _validatorUpdate;
        private ICacheProvider _cacheProvider;

        public TaskService(
            IUnitOfWork unitOfWork,
            CreateTaskValidator validator,
            ILoggingService logger,
            UpdateTaskValidator validatorUpdate,
            ICacheProvider cacheProvider)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
            _validatorUpdate = validatorUpdate;
            _cacheProvider = cacheProvider;
        }
        public TaskService(IUnitOfWork unitOfWork, ILoggingService logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        /// <summary>
        /// Delete selected task by Id (soft delete)
        /// </summary>
        /// <param name="id">Id of a task</param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            try
            {
                var taskRepo = _unitOfWork.Repository<TaskModel>();

                var task = await taskRepo.SingleOrDefaultAsync(x => x.Id == id);

                if (task == null)
                    throw new EntityNotFoundException(nameof(TaskDto), id);

                //Soft delte
                task.IsActive = false;
                task.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.Save();

                _logger.LogInforamtion("Deleted a task with Id: {id} from Delete method {repo}", task.Id, typeof(TaskService));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} Delete method", typeof(TaskService));

                throw;
            }
        }
        /// <summary>
        ///  Get all tasks.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TaskDto>> GetAll()
        {
            var tasksList =await _cacheProvider.GetCachedResponseForTasks();

            _logger.LogInforamtion("Retrived a tasks from GetAll method {repo}", typeof(TaskService));

            return tasksList.ToList();
        }
        /// <summary>
        /// Get specific task by Id.
        /// </summary>
        /// <param name="id">Id of a task</param>
        /// <returns></returns>
        public async Task<TaskDto> GetOne(int id)
        {
            try
            {
                var task = await _unitOfWork.Repository<TaskModel>().SingleOrDefaultAsync(x => x.Id == id);

                if (task == null)
                {
                    throw new EntityNotFoundException(nameof(TaskDto), id);
                }

                _logger.LogInforamtion("Retrived a task with Id: {id} from GetOne method {repo}",task.Id,typeof(TaskService));

                return new TaskDto
                {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    Priority = task.Priority,
                    Status= task.Status,
                    ProjectId = task.ProjectId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred at {repo} GetOne method", typeof(TaskService));

                throw;
            }
        }
        /// <summary>
        /// Create new task.
        /// </summary>
        /// <param name="task">New task.</param>
        /// <returns></returns>
        public async Task Insert(TaskDto task)
        {
            try
            {
                _validator.ValidateAndThrow(task);

                var t = new TaskModel()
                {
                    Name = task.Name,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    Project = await _unitOfWork.Repository<ProjectModel>().FindAsync(task.ProjectId)
                };

                await _unitOfWork.Repository<TaskModel>().InsertAsync(t, true);

                task.Id = t.Id;

                _logger.LogInforamtion("Created a tasks from Insert method {repo}", typeof(TaskService));

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} Insert method", typeof(TaskService));

                throw;
            }
           
        }
        /// <summary>
        /// Updating a task. Put method (replacing existing task)
        /// </summary>
        /// <param name="task">New task</param>
        /// <param name="id">Id of a task.</param>
        /// <returns></returns>
        public async Task Update(TaskDto task,int id)
        {
            try
            {
                task.Id = id;

                _validatorUpdate.ValidateAndThrow(task);

                var row = await _unitOfWork.Repository<TaskModel>().SingleOrDefaultAsync(x => x.Id == task.Id);

                if(row == null)
                {
                    throw new EntityNotFoundException(nameof(TaskDto),id);
                }
                row.Name= task.Name;
                row.Status= task.Status;
                row.Priority= task.Priority;
                row.Description= task.Description;
                row.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Save();

                _logger.LogInforamtion("Updated a task with an Id: {id} from Update method {repo}", task.Id, typeof(TaskService));

            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Error occurred at {repo} Update method", typeof(TaskService));

                throw;
            }
        }
    }
}
