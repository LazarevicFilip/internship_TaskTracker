﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class PagingDto
    {
        public int? Page { get; set; }
        public int? perPage { get; set; }
    }
}
