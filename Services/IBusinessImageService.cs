using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface IBusinessImageService
    {
        /// <summary>
        /// Retrieve images for specified business.
        /// </summary>
        /// <param name="businessId">Business ID.</param>
        /// <returns>Collection of business images or an error.</returns>
        Task<ServiceResult<IEnumerable<BusinessImageDto>>> GetImagesAsync(int businessId);

        /// <summary>
        /// Uploads an image to the cloud and adds it to the business gallery.
        /// </summary>
        Task<ServiceResult<BusinessImageDto>> UploadImageAsync(int businessId, CreateBusinessImageDto imageUploadDto);

        /// <summary>
        /// Delete an image from the cloud and remove it from the business gallery.
        /// </summary>
        Task<ServiceResult> DeleteImageAsync(int businessId, int imageId);

        /// <summary>
        /// Sets business image as the primary image.
        /// </summary>
        Task<ServiceResult> SetPrimaryImageAsync(int businessId, int imageId);
    }
}
