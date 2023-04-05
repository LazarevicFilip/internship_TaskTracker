using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.V1.Responses
{
    public class PagedResponse<T>
    {
        public int Page { get; init; }
        public int PerPage { get; init; }
        public int TotalCount { get; init; }
        public int PagesCount => (int)Math.Ceiling((float)TotalCount / PerPage);
        public IEnumerable<T> Data { get; init; }
    }
}
