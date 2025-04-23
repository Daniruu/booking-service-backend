﻿using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookingService.Services
{
    public class GoogleCloudStorageService : IGoogleCloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly IConfiguration _configuration;
        private readonly string? _bucketName;

        public GoogleCloudStorageService(IConfiguration configuration)
        {
            var credentialPath = configuration["GoogleCloud:CredentialPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            _storageClient = StorageClient.Create();
            _bucketName = configuration["GoogleCloud:BucketName"];
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string folder, string fileName)
        {
            var objectName = $"{folder}/{fileName}";

            try
            {
                var obj = await _storageClient.UploadObjectAsync(_bucketName, objectName, null, fileStream);
                return $"https://storage.googleapis.com/{_bucketName}/{folder}/{objectName}";
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Error while uploading a file: {ex.Message}");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(string folder, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File can not be empty.");

            var fileName = $"{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}";

            using (var stream = file.OpenReadStream())
            {
                return await UploadFileAsync(stream, folder, fileName);
            }
        }

        public async Task DeleteFileAsync(string folder, string fileName)
        {
            var objectName = $"{folder}/{fileName}";

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
                Console.WriteLine($"File {objectName} deleted successfully.");
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.Error.Code == 404)
                {
                    Console.WriteLine($"File {objectName} not found.");
                    return;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
