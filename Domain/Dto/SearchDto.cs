using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class SearchDto : PagingDto
    {
        public string? KeyWord { get; init; }
        //Results will be all projects that start after provided StartDate.
        public DateTime? StartDate { get; init; }
        //Results will be all projects that are finished before provided EndDate.
        public DateTime? EndDate { get; init; }
        public string? Name { get; init; }
        public ProjectStatus? Status { get; init; }
        public Priority? Priority { get; init; }
        public bool? SortByNameAsc { get; init; }

    }
}
