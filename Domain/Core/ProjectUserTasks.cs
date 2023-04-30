using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class ProjectUserTasks
    {
        public int TaskId { get; set; }
        public int ProjectUserId { get; set; }
        public TaskModel Task { get; set; }
        public ProjectUsers ProjectUsers { get; set; }
    }
}
