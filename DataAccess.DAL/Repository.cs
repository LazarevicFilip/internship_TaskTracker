using DataAccess.DAL.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccess.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _entities;
        private readonly DbContext _context;

        public DbSet<T> Entities => _entities;
        public DbContext Context => _context;

        public Repository(DbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await Entities.FindAsync(id);
            if (entity != null)
            {
                Entities.Remove(entity);
            }
        }
        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }
        public virtual async Task<T> FindAsync(params object[] keyValues)
        {
            return await Entities.FindAsync(keyValues);
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await Entities.ToListAsync();
        }

        public async Task InsertAsync(T entity)
        {
            await Entities.AddAsync(entity);
        }
        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities.SingleOrDefaultAsync(predicate);
        }
    }
}
