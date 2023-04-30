using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class ProjectUsers
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public User User { get; set; }
        public ProjectModel Project { get; set; }
        public IList<ProjectUserTasks> Tasks { get; set; }


    }
}
