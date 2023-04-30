using BusinessLogic.BAL.Cache;
using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Storage;
using BusinessLogic.BAL.Validators;
using BusinessLogic.BAL.Validators.ProjectsValidator;
using DataAccess.DAL.Core;
using DataAccess.DAL.Extensions;
using DataAccess.DAL.Logging;
using Domain.Core;
using Domain.Dto;
using Domain.Dto.V1;
using Domain.Dto.V1.Request;
using Domain.Dto.V1.Responses;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        private ICacheProvider<ProjectModel> _cacheProvider;
        private readonly IBlobService _blobService;

        public ProjectService(
            IUnitOfWork unitOfWork,
            ILoggingService logger,
            AddTasksDtoValidator tasksProjectValidator,
            UpdateProjectValidator updateProjectValidator,
            CreateProjectValidator createProjectValidator,
            ICacheProvider<ProjectModel> cacheProvider,
            IBlobService blobService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tasksProjectValidator = tasksProjectValidator;
            _updateProjectValidator = updateProjectValidator;
            _createProjectValidator = createProjectValidator;
            _cacheProvider = cacheProvider;
            _blobService = blobService;
        }
        /// <summary>
        /// Delete selected project by Id (soft delete)
        /// </summary>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            try
            {
                var projectRepo = _unitOfWork.Repository<ProjectModel>();

                var projectWithTasks = projectRepo.Include(x => x.Tasks);

                var project = await projectRepo.SingleOrDefaultAsync(x => x.Id == id);

                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectModel), id);
                }
                if (project.Tasks.Any())
                {
                    throw new ConflictedActionException("Cannot delete a project because it contains tasks. Tasks:" + string.Join(",", project.Tasks.Select(x => x.Name)));
                }
                //Soft delete
                project.IsActive = false;
                project.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.Save();

                _logger.LogInformation("Deleted a project with an Id: {id} from DeleteAsync method {repo}", id, typeof(ProjectService));

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} DeleteAsync method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Delete selected project by Id (for cleanups in integration test, deleting from db.)
        /// </summary>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task ForceDeleteAsync(int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsyncWithIgnoreQueryFilters(x => x.Id == id);

                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectModel), id);
                }
                _unitOfWork.Repository<ProjectModel>().Delete(project);

                await _unitOfWork.Save();

                _logger.LogInformation("Deleted a project with an Id: {id} from ForceDeleteAsync method {repo}", id, typeof(ProjectService));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} ForceDeleteAsync method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Get all projects.
        /// Perform filtering and sorting in case a dto is provided.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<PagedResponse<ProjectResponseDto>> GetAllAsync(SearchDto dto)
        {
            var projects = await preformFilteringAsync(dto);

            var cachedProjects = new List<ProjectModel>();

            if (projects.Any())
            {
                cachedProjects = (List<ProjectModel>)await _cacheProvider.GetCachedResponseAsync(nameof(ProjectService),projects, dto.Page.Value - 1, dto.perPage.Value);
            }

            _logger.LogInformation("Retrieved projects from GetAllAsync method {repo}", typeof(ProjectService));

            var projectsCount = await _unitOfWork.Repository<ProjectModel>().GetAllAsync();

            return new PagedResponse<ProjectResponseDto>
            {
                Data = cachedProjects.Select(x => new ProjectResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    CompletionDate = x.CompletionDate,
                    ProjectStatus = x.ProjectStatus,
                    ProjectPriority = x.ProjectPriority,
                    Taks = _unitOfWork.Repository<TaskModel>().Where(y => y.ProjectId == x.Id).Select(t => new TaskSummaryDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                    }).ToList()
                }),
                Page = dto.Page.Value,
                PerPage = dto.perPage.Value,
                TotalCount = projectsCount.Count
            };
        }
        /// <summary>
        /// Get specific project by Id.
        /// </summary>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task<ProjectResponseDto> GetOneAsync(int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);

                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectResponseDto), id);
                }

                _logger.LogInformation("Retrieved a project with an Id: {id} from GetOneAsync method {repo}", id, typeof(ProjectService));

                return new ProjectResponseDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    CompletionDate = project.CompletionDate,
                    ProjectStatus = project.ProjectStatus,
                    ProjectPriority = project.ProjectPriority,
                    FileURI = project.FileURI,
                    Taks = _unitOfWork.Repository<TaskModel>().Where(y => y.ProjectId == project.Id).Select(t => new TaskSummaryDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} GetOneAsync method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="dto">New project</param>
        /// <returns></returns>
        public async Task<ProjectResponseDto> InsertAsync(ProjectRequestDto dto)
        {
            try
            {
                await _createProjectValidator.ValidateAndThrowAsync(dto);

                var project = new ProjectModel()
                {
                    Name = dto.Name,
                    StartDate = dto.StartDate,
                    CompletionDate = dto.CompletionDate,
                    ProjectStatus = dto.ProjectStatus,
                    ProjectPriority = dto.ProjectPriority,
                };
                if (dto.File != null)
                {
                    var fileURI = await _blobService.UploadFileBlobAsync(dto.File);

                    project.FileURI = fileURI.uri;
                }

                await _unitOfWork.Repository<ProjectModel>().InsertAsync(project);

                await _unitOfWork.Save();

                _logger.LogInformation("Create a project from InsertAsync method {repo}", typeof(ProjectService));

                return new ProjectResponseDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    CompletionDate = project.CompletionDate,
                    FileURI = project.FileURI,
                    Taks = project.Tasks.Select(x => new TaskSummaryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).ToList(),
                    ProjectStatus = project.ProjectStatus,
                    ProjectPriority = project.ProjectPriority,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} InsertAsync method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Updating a project. Put method (replacing existing project)
        /// </summary>
        /// <param name="project">New project</param>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateProjectRequestDto project, int id)
        {
            try
            {
                project.Id = id;
                    
                await _updateProjectValidator.ValidateAndThrowAsync(project);

                var context = _unitOfWork.Repository<ProjectModel>();

                var row = await context.SingleOrDefaultAsync(x => x.Id == id);

                if (row == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectResponseDto), id);
                }
                row.Name = project.Name;
                row.ProjectStatus = project.ProjectStatus;
                row.ProjectPriority = project.ProjectPriority ?? row.ProjectPriority;
                row.StartDate = project.StartDate;
                row.CompletionDate = project.CompletionDate ?? row.CompletionDate;
                row.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Save();

                _logger.LogInformation("Update projects with an Id: {id} from UpdateAsync method {repo}", id, typeof(ProjectService));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is ProjectModel)
                    {
                        _logger.LogError(ex, "Concurrency violation occurred at {repo} UpdateAsync method", typeof(ProjectService));

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
                _logger.LogError(ex, "Error occurred at {repo} UpdateAsync method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Add provided tasks to a specified project.
        /// </summary>
        /// <param name="tasks">Array of ids that represent each task.</param>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task AddTasksToProjectAsync(AddTasksDto tasks, int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);
                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectResponseDto), id);
                }
                await _tasksProjectValidator.ValidateAndThrowAsync(tasks);
                    
                var taskModels = _unitOfWork.Repository<TaskModel>().Where(x => tasks.Tasks.Contains(x.Id));

                project.Tasks.AddRange(taskModels);

                await _unitOfWork.Save();

                _logger.LogInformation("Add tasks to the project from AddTasksToProjectAsync method {repo}", typeof(ProjectService));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} AddTasksToProjectAsync method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// Delete all provided tasks for a specified project. (soft delete) 
        /// </summary>
        /// <param name="tasks">Array of ids that represent each task.</param>
        /// <param name="id">Id of a project</param>
        /// <returns></returns>
        public async Task RemoveTasksFromProjectAsync(AddTasksDto tasks, int id)
        {
            try
            {
                var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == id);
                if (project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectResponseDto), id);
                }
                await _tasksProjectValidator.ValidateAndThrowAsync(tasks);

                var taskModels = _unitOfWork.Repository<TaskModel>().Where(x => tasks.Tasks.Contains(x.Id));

                taskModels.ForEach(x =>
                {
                    x.IsActive = false;
                    x.DeletedAt = DateTime.UtcNow;
                });

                await _unitOfWork.Save();

                _logger.LogInformation("Remove tasks from the project in RemoveTasksFromProjectAsync method {repo}", typeof(ProjectService));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} RemoveTasksFromProject method", typeof(ProjectService));

                throw;
            }
        }
        /// <summary>
        /// preform filtering and sorting based on properties in provided dto.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async Task<List<ProjectModel>> preformFilteringAsync(SearchDto dto)
        {
            if (dto.Page == null || dto.Page < 1)
            {
                dto.Page = 1;
            }
            if (dto.perPage == null || dto.perPage < 5)
            {
                dto.perPage = 5;
            }
            //building of a query, if the property of dto is not null, we include that parameter in
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
                projects = projects.Where(x => (int)x.ProjectStatus == (int)dto.Status.Value);
            }
            if (dto.Priority.HasValue)
            {
                projects = projects.Where(x => x.ProjectPriority == dto.Priority.Value);
            }
            if (dto.SortByNameAsc.HasValue)
            {
                projects = dto.SortByNameAsc.Value ? projects.OrderBy(x => x.Name) : projects.OrderByDescending(x => x.Name);
            }
            //point of materialization, we send constructed query to db
            return await projects.Skip(((dto.Page.Value - 1) * dto.perPage.Value)).Take(dto.perPage.Value).ToListAsync();
        }

        public async Task<IEnumerable<ProjectSummaryResponseDto>> GetProjectsOfUserAsync(int userId)
        {
            var userProjects =  _unitOfWork.Repository<ProjectModel>().Where(x => x.Users.Any(u => u.UserId == userId));

            var cachedUserProjects = await _cacheProvider.GetCachedResponseAsync("TasksOfPRojects", userProjects, 0, 0);

            return cachedUserProjects.Select(x => new ProjectSummaryResponseDto
            {
                Id = x.Id,
                Name = x.Name
            });
           

        }

        public async Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId)
        {
            var project = await _unitOfWork.Repository<ProjectModel>().SingleOrDefaultAsync(x => x.Id == projectId);

            if (project == null)
            {
                throw new EntityNotFoundException(nameof(ProjectResponseDto), projectId);
            }

            var tasks = _unitOfWork.Repository<TaskModel>().Include(x => x.Files).Where(x => x.ProjectId == projectId);

            var s = _unitOfWork.Repository<ProjectUserTasks>().Include(x => x.ProjectUsers).ToList();

            return tasks.Select(x => new TaskDto
            {
                Id = x.Id,
                Name = x.Name,
                ProjectId = projectId,
                Priority = x.Priority,
                Description = x.Description,
                Status = x.Status,
                UserIds = s.Where(y => y.TaskId == x.Id).Select(x => x.ProjectUsers.UserId).ToList(),
                TaskFiles = x.Files?.Select(y => new FileResponseDto
                {
                    FileName = y.FileName,
                    FileUri = y.FileUri,
                }).ToList(),
            });
        }

    }
}

