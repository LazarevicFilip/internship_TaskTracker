using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        protected DbContext DbContext { get; }
        IRepository<T> Repository<T>() where T : class;
        Task<int> Save(CancellationToken cancellationToken = default);
    }
}
