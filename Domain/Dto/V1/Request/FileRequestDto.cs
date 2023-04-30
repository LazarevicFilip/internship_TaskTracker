using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.V1.Request
{
    public class FileRequestDto
    {
        public string TaskId { get; set; }
        public IFormFile File { get; set; }
    }
}
