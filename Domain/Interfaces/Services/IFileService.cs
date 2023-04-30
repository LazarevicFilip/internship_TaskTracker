using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IFileService
    {
        Task<(string, string)> UploadTaskFileAsync(IFormFile file);
        Task DeleteTaskFileAsync(string fileName);

    }
}
