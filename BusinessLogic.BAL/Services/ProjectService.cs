using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Logging;
using BusinessLogic.BAL.Validators;
using BusinessLogic.BAL.Validators.ProjectsValidator;
using DataAccess.DAL.Core;
using DataAccess.DAL.Extensions;
using Domain.Dto;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggingService _logger;
        private readonly UpdateProjectValidator _updateProjectValidator;
        private readonly CreateProjectValidator _createProjectValidator;
        private readonly AddTasksDtoValidator _tasksProjectValidator;

        public ProjectService(
            IUnitOfWork unitOfWork,
            ILoggingService logger,
            AddTasksDtoValidator tasksProjectValidator,
            UpdateProjectValidator updateProjectValidator,
            CreateProjectValidator createProjectValidator
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tasksProjectValidator = tasksProjectValidator;
            _updateProjectValidator = updateProjectValidator;
            _createProjectValidator = createProjectValidator;
        }
        /// <summary>
        /// Delete selected project by Id (soft delete)
        /// </summary>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var projectRepo = _unitOfWork.Repository<ProjectModel>();

                var projectWithTasks = projectRepo.Include(x => x.Tasks);

                var project = await projectWithTasks.SingleOrDefaultAsync(x => x.Id == id);

                if(project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectModel), id);
                }
                if (project.Tasks.Any())
                {
                    throw new ConflictedActionException("Cannot delete a project because it contains tasks. Tasks:" + string.Join(",",project.Tasks.Select(x => x.Name)));
                }
                //Soft delete
                project.IsActive= false;
                project.DeletedAt = DateTime.UtcNow;

                _logger.LogInforamtion("Deleted a project with Id: {id} from Delete method {repo}", id, typeof(ProjectService));

                await _unitOfWork.CommitTransaction();

            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} Delete method", typeof(ProjectService));

                await _unitOfWork.RollbackTransaction();

                throw;
            }
        }
        /// <summary>
        /// Delete selected project by Id (for cleanups in intefration test, deleting from db.)
        /// </summary>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task forceDelete(int id)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var project = await _unitOfWork.Repository<ProjectModel>().IgnoreQueryFilters<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);

                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectModel), id);
                }
                
               await _unitOfWork.Repository<ProjectModel>().DeleteAsync(project);
                
                _logger.LogInforamtion("Deleted a project with Id: {id} from forceDelete method {repo}", id, typeof(ProjectService));

                await _unitOfWork.CommitTransaction();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} forceDelete method", typeof(ProjectService));

                await _unitOfWork.RollbackTransaction();

                throw;
            }
        }
        /// <summary>
        /// Get all projects.
        /// Perform filtering and sorting in case a dto is provided.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IList<ProjectDto>> GetAll(SearchDto dto)
        {
           var projects = await preformFiltering(dto);

            _logger.LogInforamtion("Retrived projects from GetAll method {repo}", typeof(ProjectService));

            return projects.Select(x => new ProjectDto
            {
                Id = x.Id,
                Name= x.Name,
                StartDate= x.StartDate,
                CompletionDate= x.CompletionDate,
                ProjectStatus= x.ProjectStatus,
                ProjectPriotiry = x.ProjectPriotiry,
                Taks = _unitOfWork.Repository<TaskModel>().Where(y => y.ProjectId == x.Id).Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Name= t.Name,
                }).ToList()
            }).ToList();
        }
        /// <summary>
        /// Get specific project by Id.
        /// </summary>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task<ProjectDto> GetOne(int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);

                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectDto), id);
                }

                _logger.LogInforamtion("Retrived a project with an Id: {id} from GetOne method {repo}",id, typeof(ProjectService));

                return new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    CompletionDate = project.CompletionDate,
                    ProjectStatus = project.ProjectStatus,
                    ProjectPriotiry = project.ProjectPriotiry,
                    Taks = _unitOfWork.Repository<TaskModel>().Where(y => y.ProjectId == project.Id).Select(t => new TaskSummaryDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred at {repo} GetOne method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Create new project
        /// </summary>
        /// <param name="dto">New project</param>
        /// <returns></returns>
        public async Task Insert(ProjectDto dto)
        {
            try
            {
                _createProjectValidator.ValidateAndThrow(dto);

                await _unitOfWork.BeginTransaction();

                var project = new ProjectModel()
                {
                    Name = dto.Name,
                    StartDate = dto.StartDate,
                    CompletionDate = dto.CompletionDate,
                    ProjectStatus = dto.ProjectStatus,
                    ProjectPriotiry = dto.ProjectPriotiry,
                };
                await _unitOfWork.Repository<ProjectModel>().InsertAsync(project);

                dto.Id = project.Id;

                _logger.LogInforamtion("Create a project from Insert method {repo}", typeof(ProjectService));

                await _unitOfWork.CommitTransaction();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error occurred at {repo} Insert method",typeof(ProjectService));

                await _unitOfWork.RollbackTransaction();

                throw;
            }
        }
        /// <summary>
        /// Updating a project. Put method (replacing existing project)
        /// </summary>
        /// <param name="project">New project</param>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task Update(ProjectDto project,int id)
        {
            try
            {
                project.Id = id;
                _updateProjectValidator.ValidateAndThrow(project);

                await _unitOfWork.BeginTransaction();

                var row = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == project.Id);

                if (row == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectDto), id);
                }
                row.Name = project.Name;
                row.ProjectStatus = project.ProjectStatus;
                row.ProjectPriotiry = project.ProjectPriotiry;
                row.StartDate = project.StartDate;
                row.CompletionDate = project.CompletionDate;
                row.UpdatedAt = DateTime.UtcNow;

                _logger.LogInforamtion("Update projects with an Id: {id} from Update method {repo}",id, typeof(ProjectService));

                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} Update method", typeof(TaskService));

                await _unitOfWork.RollbackTransaction();

                throw;
            }
        }
        /// <summary>
        /// Add all provided tasks to a specified project.
        /// </summary>
        /// <param name="tasks">Array of ids that represent each task.</param>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task AddTasksToProject(AddTasksDto tasks, int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);
                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectDto), id);
                }
                _tasksProjectValidator.ValidateAndThrow(tasks);

                await _unitOfWork.BeginTransaction();

                var taskModels = _unitOfWork.Repository<TaskModel>().Where(x => tasks.Tasks.Contains(x.Id));

                project.Tasks.AddRange(taskModels);

                _logger.LogInforamtion("Add tasks to project from AddTasksToProject method {repo}", typeof(ProjectService));

                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} AddTasksToProject method", typeof(ProjectService));

                await _unitOfWork.RollbackTransaction();

                throw;
            }
        }
        /// <summary>
        /// Delete all provided tasks for specified project. (soft delete) 
        /// </summary>
        /// <param name="tasks">Array of ids that represent each task.</param>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task RemoveTasksFromProject(AddTasksDto tasks, int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);
                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectDto), id);
                }
                _tasksProjectValidator.ValidateAndThrow(tasks);

                await _unitOfWork.BeginTransaction();

                var taskModels = await _unitOfWork.Repository<TaskModel>().Where(x => tasks.Tasks.Contains(x.Id)).ToListAsync();

                taskModels.ForEach(x =>
                {
                    x.IsActive = false;
                    x.DeletedAt = DateTime.UtcNow;
                });

                _logger.LogInforamtion("Remove tasks from project in RemoveTasksFromProject method {repo}", typeof(ProjectService));

                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred at {repo} RemoveTasksFromProject method", typeof(ProjectService));

                await _unitOfWork.RollbackTransaction();

                throw;
            }
        }
        /// <summary>
        /// preform filtering and sorting based on properties in provided dto.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public  async Task<IList<ProjectModel>> preformFiltering(SearchDto dto)
        {
            //building of a query, if property of dto is not null, we include that parameter in query
            var projects = _unitOfWork.Repository<ProjectModel>().AsQueryable();
            if (dto.StartDate.HasValue)
            {
                projects = projects.Where(x => DateTime.Compare(x.StartDate, (DateTime)dto.StartDate.Value) >= 0);
            }
            if (dto.EndDate.HasValue)
            {
                projects = projects.Where(x => x.CompletionDate != null && DateTime.Compare((DateTime)x.CompletionDate, (DateTime)dto.EndDate.Value) <= 0);
            }
            if (!string.IsNullOrEmpty(dto.KeyWord))
            {
                projects = projects.Where(x => x.Name.ToLower().Contains(dto.KeyWord.ToLower()!));
            }
            if (dto.Status.HasValue)
            {
                projects = projects.Where(x => (int)x.ProjectStatus == dto.Status.Value);
            }
            if (dto.Priority.HasValue)
            {
                projects = projects.Where(x => x.ProjectPriotiry == dto.Priority.Value);
            }
            if (dto.SortByNameAsc.HasValue)
            {
               projects =  dto.SortByNameAsc.Value ? projects.OrderBy(x => x.Name) : projects.OrderByDescending(x => x.Name);
            }
            //point of materialization, we send constructed query to db
            return await projects.ToListAsync();
        }
    }
}

