using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DAL.Core;
using DataAccess.DAL;
using Domain.Dto;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BusinessLogic.BAL.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private static readonly SemaphoreSlim GetUsersSemaphore = new SemaphoreSlim(1, 1);
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private const string ProjectsListCacheKeys = "TasksList";

        public CacheProvider(IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TaskDto>> GetCachedResponseForTasks()
        {
            try
            {
                return await GetCachedResponse(ProjectsListCacheKeys, GetUsersSemaphore);
            }
            catch(Exception)
            {
                throw;
            }
        }
        private async Task<IEnumerable<TaskDto>> GetCachedResponse(string cacheKey, SemaphoreSlim semaphore)
        {
            bool isAvailable = _cache.TryGetValue(cacheKey, out List<TaskDto> tasks);
            if (isAvailable)
            {
                return tasks;
            }
            try
            {
                await semaphore.WaitAsync();
                isAvailable = _cache.TryGetValue(cacheKey, out tasks);
                if (isAvailable) return tasks;
                var tasks2 = await _unitOfWork.Repository<TaskModel>().GetAllAsync();
                tasks = tasks2.Select(x => new TaskDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Priority = x.Priority,
                    Status = x.Status,
                    ProjectId = x.ProjectId
                }).ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Size = 1024,
                };
                _cache.Set(cacheKey, tasks, cacheEntryOptions);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                semaphore.Release();
            }
            return tasks;
        }



    }
}
