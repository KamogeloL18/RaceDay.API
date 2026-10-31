using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RaceDay.API.Services
{
    public class BlobService : IBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName = "raceday-images";

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Create container if it doesn't exist
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Generate unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            // Upload the file
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            // Return the URL
            return blobClient.Uri.ToString();
        }
    }
}