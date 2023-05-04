using BusinessLogic.BAL.Cache;
using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Storage;
using BusinessLogic.BAL.Validators;
using BusinessLogic.BAL.Validators.TaskValidators;
using DataAccess.DAL.Core;
using DataAccess.DAL.Extensions;
using DataAccess.DAL.Logging;
using Domain.Core;
using Domain.Dto;
using Domain.Dto.V1.Request;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggingService _logger;
        private readonly CreateTaskValidator _validator;
        private readonly UpdateTaskValidator _validatorUpdate;
        private readonly FileRequestDtoValidator _validatorTaskWithFile;
        private ICacheProvider<TaskModel> _cacheProvider;


        public TaskService(
            IUnitOfWork unitOfWork,
            CreateTaskValidator validator,
            ILoggingService logger,
            UpdateTaskValidator validatorUpdate,
            ICacheProvider<TaskModel> cacheProvider,
            FileRequestDtoValidator validatorTaskWithFile)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
            _validatorUpdate = validatorUpdate;
            _cacheProvider = cacheProvider;
            _validatorTaskWithFile = validatorTaskWithFile;
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
        public async Task DeleteAsync(int id)
        {
            try
            {
                var task  =await  _unitOfWork.Repository<TaskModel>().SingleOrDefaultAsync(x => x.Id == id);

                if (task == null)
                    throw new EntityNotFoundException(nameof(TaskDto), id);

                //Soft delte
                task.IsActive = false;
                task.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.Save();

                _logger.LogInformation("Deleted a task with an Id: {id} from DeleteAsync method {repo}", task.Id, typeof(TaskService));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} DeleteAsync method", typeof(TaskService));

                throw;
            }
        }
        /// <summary>
        ///  Get all tasks.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TaskDto>> GetAllAsync(PagingDto dto)
        {
            var tasks = await PreformPagingAsync(dto);

            _logger.LogInformation("Retrieved a tasks from GetAllAsync method {repo}", typeof(TaskService));

            return tasks.ToList();
        }
        /// <summary>
        /// Get specific task by an Id.
        /// </summary>
        /// <param name="id">Id of a task</param>
        /// <returns></returns>
        public async Task<TaskDto> GetOneAsync(int id)
        {
            try
            {
                var task = await _unitOfWork.Repository<TaskModel>().SingleOrDefaultAsync(x => x.Id == id);

                if (task == null)
                {
                    throw new EntityNotFoundException(nameof(TaskDto), id);
                }

                _logger.LogInformation("Retrieved a task with an Id: {id} from GetOneAsync method {repo}", task.Id,typeof(TaskService));

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
                _logger.LogError(ex, "Error occurred at {repo} GetOneAsync method", typeof(TaskService));

                throw;
            }
        }
        /// <summary>
        /// Create new task.
        /// </summary>
        /// <param name="task">New task.</param>
        /// <returns></returns>
        public async Task InsertAsync(TaskDto task)
        {
            try
            {
                await _validator.ValidateAndThrowAsync(task);

                var t = new TaskModel()
                {
                    Name = task.Name,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    Project = await _unitOfWork.Repository<ProjectModel>().FindAsync(task.ProjectId)
                };

                await _unitOfWork.Repository<TaskModel>().InsertAsync(t);

                foreach (var u in task.UserIds)
                {
                    var user = await _unitOfWork.Repository<ProjectUsers>().SingleOrDefaultAsync(x => x.ProjectId == t.ProjectId && x.UserId == u);
                    await _unitOfWork.Repository<ProjectUserTasks>().InsertAsync(new ProjectUserTasks
                    {
                        ProjectUserId = user.Id,
                        Task = t
                    });
                }
                await _unitOfWork.Save();

                task.Id = t.Id;

                _logger.LogInformation("Created a task from InsertAsync method {repo}", typeof(TaskService));

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} InsertAsync method", typeof(TaskService));

                throw;
            }
           
        }
        /// <summary>
        /// Updating a task. Put method (replacing existing task)
        /// </summary>
        /// <param name="task">New task</param>
        /// <param name="id">Id of a task.</param>
        /// <returns></returns>
        public async Task UpdateAsync(TaskDto task,int id)
        {
            try
            {
                task.Id = id;

                await _validatorUpdate.ValidateAndThrowAsync(task);

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

                var userAlreadyOnTask = _unitOfWork.Repository<ProjectUserTasks>().Where(x => x.TaskId == task.Id);
                userAlreadyOnTask.ForEach(x =>
                {
                    _unitOfWork.Repository<ProjectUserTasks>().Delete(x);
                });

                foreach (var u in task.UserIds)
                {
                    var user = await _unitOfWork.Repository<ProjectUsers>().SingleOrDefaultAsync(x => x.ProjectId == row.ProjectId && x.UserId == u);
                   
                  
                  
                    await _unitOfWork.Repository<ProjectUserTasks>().InsertAsync(new ProjectUserTasks
                    {
                        ProjectUserId = user.Id,
                        Task = row
                    });
                }

                await _unitOfWork.Save();

                _logger.LogInformation("Updated a task with an Id: {id} from UpdateAsync method {repo}", task.Id, typeof(TaskService));

            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is TaskModel)
                    {
                        _logger.LogError(ex, "Concurrency violation occurred at {repo} UpdateAsync method", typeof(TaskService));

                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());

                        throw;
                    }
                    else
                    {
                        throw new NotSupportedException("Unable to handle concurrency conflicts for " + entry.Metadata.Name);
                    }
                }
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Error occurred at {repo} UpdateAsync method", typeof(TaskService));

                throw;
            }
        }
        private async Task<IEnumerable<TaskDto>> PreformPagingAsync(PagingDto dto)
        {
            if (dto.Page == null || dto.Page < 1)
            {
                dto.Page = 1;
            }
            if (dto.perPage == null || dto.perPage < 5)
            {
                dto.perPage = 5;
            }

            var tasksList = await _unitOfWork.Repository<TaskModel>().ToListAsync();

            var tasks = tasksList.Skip(((dto.Page.Value - 1) * dto.perPage.Value)).Take(dto.perPage.Value).ToList();

            var cachedTasks = await _cacheProvider.GetCachedResponseAsync(nameof(TaskService), tasks, dto.Page.Value,dto.perPage.Value);

            return tasksList.Select(x => new TaskDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Priority = x.Priority,
                Status = x.Status,
                ProjectId = x.ProjectId
            });
        }

        public async Task InsertTaskFilesAsync(FileRequestDto dto,string fileUri, string newFileName)
        {
            try
            {
                await _validatorTaskWithFile.ValidateAndThrowAsync(dto);

                var taskFileModel = new TaskFiles
                {
                    FileName = newFileName,
                    ContentType = dto.File.ContentType,
                    FileUri = fileUri,
                    TaskId = int.Parse(dto.TaskId)
                };
                await _unitOfWork.Repository<TaskFiles>().InsertAsync(taskFileModel);
                await _unitOfWork.Save();

                _logger.LogInformation("Created a taskFileModel from InsertTaskFiles method {repo}", typeof(TaskService));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} InsertTaskFiles method", typeof(TaskService));

                throw;
            }
        }

        public async Task DeleteTaskFilesAsync(int taskId, string fileName)
        {
            try
            {
                var fileToDelete = await _unitOfWork.Repository<TaskFiles>().SingleOrDefaultAsync(x => x.FileName == fileName && x.TaskId == taskId);
                if (fileToDelete == null)
                {
                    throw new EntityNotFoundException(nameof(TaskFiles), taskId);
                }
                await _unitOfWork.Repository<TaskFiles>().DeleteAsync(fileToDelete.Id);
                await _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} DeleteTaskFilesAsync method", typeof(TaskService));

                throw;

            }
        }
        public List<FileResponseDto> GetTaskFiles(int taskId)
        {
            var files =  _unitOfWork.Repository<TaskFiles>().Where(x => x.TaskId == taskId).Select(x => new FileResponseDto
            {
                FileName = x.FileName,
                FileUri = x.FileUri,
            }).ToList();

            return files;
        }
    }
}
