using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL.Core
{
    public enum Status
    {
        ToDo,
        InProgress,
        Done
    }
    public class TaskModel : BaseEntity
    {
        public string Name { get; set; }
        public Status? Status { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public ProjectModel Project { get; set; }

    }
}
