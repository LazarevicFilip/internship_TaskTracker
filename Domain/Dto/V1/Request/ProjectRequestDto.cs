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
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public Priority? ProjectPriority { get; set; }
        public IFormFile? File { get; set; }
    }
}
