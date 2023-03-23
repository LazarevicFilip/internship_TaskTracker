using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.V1.Responses
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalCount { get; set; }
        public int PagesCount => (int)Math.Ceiling((float)TotalCount / PerPage);
        public IEnumerable<T> Data { get; set; }
    }
}
