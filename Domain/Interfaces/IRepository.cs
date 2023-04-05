using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> Entities { get; }
        DbContext Context { get; }
        Task<IList<T>> GetAllAsync();
        Task<T> FindAsync(params object[] keyValues);
        Task InsertAsync(T entity);
        Task DeleteAsync(int id);
        void Delete(T entity);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}
