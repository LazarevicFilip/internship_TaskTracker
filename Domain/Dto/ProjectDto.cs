﻿using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public Priority? ProjectPriority { get; set; }
        public List<TaskSummaryDto>? Taks { get; set; } = new();
    }
}
