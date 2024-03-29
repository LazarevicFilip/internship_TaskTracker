﻿using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Cache
{
    public interface ICacheProvider<T>
    {
        public Task<IEnumerable<T>> GetCachedResponseAsync(string keyName, List<T> data, int page, int perPage);
    }
}
