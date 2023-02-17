using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Cache
{
    public interface ICacheProvider
    {
        public Task<IEnumerable<TaskDto>> GetCachedResponseForTasks();
    }
}
