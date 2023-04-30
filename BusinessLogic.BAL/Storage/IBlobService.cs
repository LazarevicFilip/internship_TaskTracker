using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Storage
{
    public interface IBlobService
    {
        public Task<(string uri, string newFileName)> UploadFileBlobAsync(IFormFile file);
        public Task DeleteFileBlobAsync(string fileName);
    }
}
