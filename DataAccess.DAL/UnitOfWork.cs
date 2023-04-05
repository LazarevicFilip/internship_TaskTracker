using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private object _lockObject = new object();

        public DbContext DbContext => _context;
        private Dictionary<string, object> Repositories { get; }

        public UnitOfWork(TaskContext context)
        {
            _context = context;
            Repositories = new Dictionary<string, dynamic>();
        }
        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            var typeName = type.Name;
            lock (_lockObject)
            {
                if (Repositories.ContainsKey(typeName))
                {
                    return (IRepository<T>)Repositories[typeName];
                }
                var repository = new Repository<T>(DbContext);
                Repositories.Add(typeName, repository);
                return repository;
            }
        }
        public async Task<int> Save(CancellationToken cancellationToken = default)
        {
            return await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
