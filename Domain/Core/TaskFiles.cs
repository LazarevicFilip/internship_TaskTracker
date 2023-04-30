using DataAccess.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class TaskFiles
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUri { get; set; }
        public string ContentType { get; set; }
        public int TaskId { get; set; }
        public TaskModel Task { get;set; }
    }
}
