using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.V1.Request
{
    public class UpdateProjectRequestDto
    {
        public int Id { get; set; }
        public string Name { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? CompletionDate { get; init; }
        public ProjectStatus ProjectStatus { get; init; }
        public Priority? ProjectPriority { get; init; }
        public List<int> UserIds { get; init; }
    }
}
