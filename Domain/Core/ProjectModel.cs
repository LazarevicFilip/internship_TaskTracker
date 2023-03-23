using Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public enum Priority
    {
        Low,
        Middle,
        High,
        VeryHigh
    }
    public class ProjectModel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }  
        public ProjectStatus ProjectStatus { get; set; }
        public Priority? ProjectPriority { get; set; }
        public string? FileURI { get; set; }
        public List<TaskModel> Tasks { get; set; } = new();
        public byte[] RowVersion { get; set; }
    }
}
