using BusinessLogic.BAL.Exceptions;
using BusinessLogic.BAL.Validators;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectService> _logger;
        private readonly ProjectValidator _validator;
        public ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger, ProjectValidator validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validator = validator;
        }

        public async Task Delete(int id)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                var projectRepo = _unitOfWork.Repository<ProjectModel>();
                var projectWithTasks = projectRepo.Include(x => x.Taks);
                var project = await projectWithTasks.FirstOrDefaultAsync(x => x.Id == id);
                if(project == null)
                {
                    throw new EntityNotFoundException(nameof(ProjectModel), id);
                }
                if (project.Taks.Any())
                {
                    throw new ConflictedActionException("Cannot delete a project because it contains tasks. Tasks:" + string.Join(",",project.Taks.Select(x => x.Name)));
                }
                //await projectRepo.DeleteAsync(project);
                project.IsActive= false;
                project.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.CommitTransaction();
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} GetOne method", typeof(ProjectService));
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<IList<ProjectDto>> GetAll(SearchDto dto)
        {
           var projects = await preformFiltering(dto);
          
            return projects.Select(x => new ProjectDto
            {
                Id = x.Id,
                Name= x.Name,
                StartDate= x.StartDate,
                CompletionDate= x.CompletionDate,
                ProjectStatus= x.ProjectStatus,
                ProjectPriotiry = x.ProjectPriotiry,
                Taks = _unitOfWork.Repository<TaskModel>().Where(y => y.ProjectId == x.Id).Select(t => new TaskDto
                {
                    Name= t.Name,
                    Status= t.Status,
                    Priority= t.Priority,
                    Description= t.Description,
                    ProjectId= t.ProjectId,
                }).ToList(),
            }).ToList();
        }

        public async Task<ProjectDto> GetOne(int id)
        {
            return await _unitOfWork.Repository<ProjectDto>().FindAsync(id);
        }

        public async Task Insert(ProjectDto dto)
        {
            try
            {
                //validation
                _validator.ValidateAndThrow(dto);

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
                await _unitOfWork.CommitTransaction();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred at {repo} GetOne method",typeof(ProjectService));
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public Task Update(ProjectDto project)
        {
            throw new NotImplementedException();
        }
        public async Task<IList<ProjectModel>> preformFiltering(SearchDto dto)
        {

            var projects = _unitOfWork.Repository<ProjectModel>().AsQueryable();
            if (dto.StartDate != null)
            {
                projects = projects.Where(x => DateTime.Compare(x.StartDate, (DateTime)dto.StartDate) >= 0);
            }
            if(dto.EndDate != null)
            {
                projects = projects.Where(x => x.CompletionDate != null && DateTime.Compare((DateTime)x.CompletionDate, (DateTime)dto.EndDate) <= 0);
            }
            if (!string.IsNullOrEmpty(dto.KeyWord))
            {
                projects = projects.Where(x => x.Name.Contains(dto.KeyWord!));
            }
            if(dto.Status != null)
            {
                projects = projects.Where(x => (int)x.ProjectStatus == dto.Status);
            }
            if(dto.Priority != null)
            {
                projects = projects.Where(x => x.ProjectPriotiry == dto.Priority);
            }
            return await projects.ToListAsync();
        }
    }
}
