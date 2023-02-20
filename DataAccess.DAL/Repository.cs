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
        public DbSet<T> Entities => Context.Set<T>();

        public DbContext Context { get; set; }
        public Repository(DbContext context)
        {
            Context = context;
        }
        public async Task DeleteAsync(int id, bool saveChanges = true)
        {
            var entity = await Entities.FindAsync(id);
            if (entity != null)
            {
                Entities.Remove(entity);
            }
            if (saveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(T entity, bool saveChanges = true)
        {
            Entities.Remove(entity);
            if (saveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }
        public virtual async Task<T> FindAsync(params object[] keyValues)
        {
            return await Entities.FindAsync(keyValues);
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await Entities.ToListAsync();
        }

        public async Task InsertAsync(T entity, bool saveChanges = true)
        {
            await Entities.AddAsync(entity);
            if (saveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }
        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities.SingleOrDefaultAsync(predicate);
        }
    }
}
