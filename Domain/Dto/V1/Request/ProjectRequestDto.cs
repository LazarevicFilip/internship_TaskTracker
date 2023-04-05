using DataAccess.DAL.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.V1.Request
{
    public class ProjectRequestDto
    {
        public string Name { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? CompletionDate { get; init; }
        public ProjectStatus ProjectStatus { get; init; }
        public Priority? ProjectPriority { get; init; }
        public IFormFile? File { get; init; }
    }
}
