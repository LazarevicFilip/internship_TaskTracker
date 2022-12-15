using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Task InsertAsync(T entity, bool saveChanges = true);
        Task InsertRangeAsync(IEnumerable<T> entities, bool saveChanges = true);
        Task DeleteAsync(int id, bool saveChanges = true);
        Task DeleteAsync(T entity, bool saveChanges = true);
        Task DeleteRangeAsync(IEnumerable<T> entities, bool saveChanges = true);
       
    }
}
