using BusinessLogic.BAL.Storage;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Services
{
    public class FileService : IFileService
    {
        private readonly IBlobService _blobService;

        public FileService(IBlobService blobService)
        {
            _blobService = blobService;
        }

        public async Task DeleteTaskFileAsync(string fileName)
        {
           await _blobService.DeleteFileBlobAsync(fileName);
        }

        public async Task<(string,string)> UploadTaskFileAsync(IFormFile file)
        {
            var uploadedFile = await _blobService.UploadFileBlobAsync(file);

            return (uploadedFile.Item1,uploadedFile.Item2);
        }
    }
}
