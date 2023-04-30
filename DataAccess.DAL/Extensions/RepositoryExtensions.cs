using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<bool> AnyAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        {
            return await repository.Entities.AnyAsync(predicate);
        }
        public static List<T> Where<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
            where T : class
        {
            return  repository.Entities.Where(predicate).ToList();
        }
        public static IEnumerable<TResult> Select<T, TResult>(this IRepository<T> repository, Expression<Func<T, TResult>> selector)
            where T : class
        {
            return repository.Entities.Select(selector).ToList();
        }
        public static IEnumerable<T> Include<T, TProperty>(this IRepository<T> repository, Expression<Func<T, TProperty>> path)
            where T : class
        {
            return repository.Entities.Include(path).ToList();
        }
        public static IQueryable<T> AsQueryable<T>(this IRepository<T> repository) where T : class
        {
            return repository.Entities.AsQueryable();
        }
        public static async Task<List<T>> ToListAsync<T>(this IRepository<T> repository)
            where T : class
        {
            return await repository.Entities.ToListAsync();
        }
        //public  static  async Task<T> SingleOrDefaultAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate)
        //    where T : class
        //{
        //    return await repository.Entities.SingleOrDefaultAsync(predicate);
        //}
        public static async Task<T> SingleOrDefaultAsyncWithIgnoreQueryFilters<T>(this IRepository<T> repository, Expression<Func<T,bool>> predicate) where T : class
        {
            return await repository.Entities.IgnoreQueryFilters().SingleOrDefaultAsync(predicate);
        }

    }
}