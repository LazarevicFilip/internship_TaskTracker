using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class SearchDto
    {
        public string? KeyWord { get; set; }
        //Results will be all projects that start after provided StartDate.
        public DateTime? StartDate { get; set; }
        //Results will be all projects that are finished before provided EndDate.
        public DateTime? EndDate { get; set; }
        public string? Name { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public bool? SortByNameAsc { get; set; }

    }
}
