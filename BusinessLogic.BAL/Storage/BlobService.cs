using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BusinessLogic.BAL.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BAL.Storage
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AzureStorageOptions _blobOptions;

        public BlobService(BlobServiceClient blobServiceClient, AzureStorageOptions blobOptions)
        {
            _blobServiceClient = blobServiceClient;
            _blobOptions = blobOptions;
        }

        public async Task<string> UploadFileBlobAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.AzureBlobStorageContainer);

            var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var blobClient = containerClient.GetBlobClient(newFileName);

            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
            return blobClient.Uri.AbsoluteUri;
        }
    }
}
