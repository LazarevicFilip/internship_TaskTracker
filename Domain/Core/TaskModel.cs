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
        public string Name { get; set; } = string.Empty;
        public Status Status { get; set; }
        public string? Description { get; set; }
        public Priority? Priority { get; set; }
        public ProjectModel Project { get; set; } = new ProjectModel();
        public int ProjectId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
