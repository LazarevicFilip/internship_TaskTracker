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
        public int? Id { get; set; }
        public string Name { get; init; } = string.Empty;
        public Status Status { get; init; }
        public string? Description { get; init; }
        public Priority? Priority { get; set; }
        public int ProjectId { get; init; }
        public List<int> UserIds { get; init; }
        public List<FileResponseDto>? TaskFiles { get; init; }

    }
    public class FileResponseDto
    {
        public string FileName { get; init; }
        public string FileUri { get; init; }
    }
    
}
