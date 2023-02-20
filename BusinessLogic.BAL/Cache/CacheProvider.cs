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
    public class CacheProvider<T> : ICacheProvider<T> where T : class
    {
        private static readonly SemaphoreSlim GetUsersSemaphore = new SemaphoreSlim(1, 1);
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public CacheProvider(IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<T>> GetCachedResponseForTasks(string keyName, int page = 1, int perPage = 5)
        {
            try
            {
                var cacheKeyWithQueryString = $"{keyName}_{page}_{perPage}";

                return await GetCachedResponse(cacheKeyWithQueryString, GetUsersSemaphore);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        private async Task<IEnumerable<T>> GetCachedResponse(string cacheKey, SemaphoreSlim semaphore)
        {
            bool isAvailable = _cache.TryGetValue(cacheKey, out List<T> tasks);
            if (isAvailable)
            {
                return tasks;
            }
            try
            {
                await semaphore.WaitAsync();

                isAvailable = _cache.TryGetValue(cacheKey, out tasks);

                if (isAvailable) return tasks;

                var type = typeof(T);

                var tasks2 = await _unitOfWork.Repository<type>().GetAllAsync();

                //tasks = tasks2.Select(x => new TaskDto
                //{
                //    Id = x.Id,
                //    Name = x.Name,
                //    Description = x.Description,
                //    Priority = x.Priority,
                //    Status = x.Status,
                //    ProjectId = x.ProjectId
                //}).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Size = 1024,
                };
                _cache.Set(cacheKey, tasks, cacheEntryOptions);
            }
            catch (Exception)
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
