using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Status Status { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int ProjectId { get; set; }
    }
}
