using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface IBusinessImageService
    {
        /// <summary>
        /// Uploads an image to the cloud and adds it to the business gallery.
        /// </summary>
        Task<ServiceResult<BusinessImageDto>> UploadImageAsync(int businessId, BusinessImageUploadDto imageUploadDto);

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
