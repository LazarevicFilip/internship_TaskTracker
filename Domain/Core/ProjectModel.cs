using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Core
{
    public enum ProjectStatus
    {
        NotStarted,
        Active,
        Completed
    }
    public class ProjectModel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }  
        public ProjectStatus ProjectStatus { get; set; }
        public int? ProjectPriotiry { get; set; }
        public List<TaskModel> Tasks { get; set; } = new();
    }
}
