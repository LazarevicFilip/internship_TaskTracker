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
using DataAccess.DAL.Extensions;

namespace BusinessLogic.BAL.Cache
{
    public class CacheProvider<T> : ICacheProvider<T> where T : class
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public CacheProvider(IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<T>> GetCachedResponseAsync(string cacheKey,int page = 1, int perPage = 5)
        {
            var cacheKeyWithQueryString = $"{cacheKey}_{page}_{perPage}";
            bool isAvailable = _cache.TryGetValue(cacheKeyWithQueryString, out IList<T>? items);
            if (isAvailable)
            {
                return items;
            }
            try
            {
                await _semaphore.WaitAsync();

                isAvailable = _cache.TryGetValue(cacheKeyWithQueryString, out items);

                if (isAvailable) return items;

                items = await _unitOfWork.Repository<T>().GetAllAsync();

                items = items.Skip(((page - 1) * perPage)).Take(perPage).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Size = 1024,
                };
                _cache.Set(cacheKeyWithQueryString, items, cacheEntryOptions);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
            return items;
        }



    }
}
