namespace BookingService.Services
{
    public interface IGoogleCloudStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string folder, string fileName);
        Task<string> UploadFileAsync(string folder, IFormFile file);
        Task DeleteFileAsync(string folder, string fileName);
    }
}
