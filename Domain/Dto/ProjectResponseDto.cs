using DataAccess.DAL.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class ProjectResponseDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? CompletionDate { get; init; }
        public ProjectStatus ProjectStatus { get; init; }
        public Priority? ProjectPriority { get; init; }
        public string? FileURI { get; init; }
        public List<TaskSummaryDto>? Taks { get; init; } = new();
    }
}
