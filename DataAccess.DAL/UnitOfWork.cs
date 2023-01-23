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
        public DbContext DbContext { get; private set; }
        private Dictionary<string, object> Repositories { get; }
        private IDbContextTransaction _transaction;
        private IsolationLevel? _isolationLevel;
        public UnitOfWork(TaskContext context)
        {
            DbContext = context;
            Repositories = new Dictionary<string, dynamic>();
        }
        private async Task StartNewTransactionIfNeeded()
        {
            if (_transaction == null)
            {
                _transaction = _isolationLevel.HasValue ? await DbContext.Database.BeginTransactionAsync(_isolationLevel.GetValueOrDefault()) : await DbContext.Database.BeginTransactionAsync();

            }
        }
        public async Task BeginTransaction()
        {
            await StartNewTransactionIfNeeded();
        }

        public async Task CommitTransaction()
        {
            await DbContext.SaveChangesAsync();
            if (_transaction == null) return;
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            if (DbContext == null)
                return;
         
            if (DbContext.Database.GetDbConnection().State == ConnectionState.Open)
            {
                DbContext.Database.GetDbConnection().Close();
            }
            DbContext.Dispose();

            DbContext = null;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            var typeName = type.Name;
            lock (Repositories)
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

        public async Task RollbackTransaction()
        {
            if (_transaction == null) return;

            await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task<int> Save(CancellationToken cancellationToken = default)
        {
            return await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
