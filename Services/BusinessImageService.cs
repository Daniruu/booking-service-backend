using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
using BookingService.Utils;

namespace BookingService.Services
{
    public class BusinessImageService : IBusinessImageService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessImageService> _logger;

        public BusinessImageService(
            IBusinessRepository businessRepository, 
            IGoogleCloudStorageService googleCloudStorageService, 
            IMapper mapper, 
            ILogger<BusinessImageService> logger)
        {
            _businessRepository = businessRepository;
            _googleCloudStorageService = googleCloudStorageService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Uploads an image to the cloud and adds it to the business gallery.
        /// </summary>
        public async Task<ServiceResult<BusinessImageDto>> UploadImageAsync(int businessId, BusinessImageUploadDto dto)
        {
            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeImages = true
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found.", businessId);
                return ServiceResult<BusinessImageDto>.Failure("Business not found.", 404);
            }

            if (business.Images.Count >= 10)
                return ServiceResult<BusinessImageDto>.Failure("Maximum number of images reached (10).", 400);

            var fileUrl = await _googleCloudStorageService.UploadFileAsync("business-images", dto.File);
            if (string.IsNullOrEmpty(fileUrl))
                return ServiceResult<BusinessImageDto>.Failure("Image upload failed.", 500);

            var image = new BusinessImage
            {
                BusinessId = businessId,
                Url = fileUrl,
                AltText = string.IsNullOrWhiteSpace(dto.AltText) ? $"Image of {business.Name}" : dto.AltText,
                IsPrimary = business.Images == null || !business.Images.Any(),
            };

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Image uploaded for business {BusinessId}: {Url}", businessId, fileUrl);
            var imageDto = _mapper.Map<BusinessImageDto>(image);

            return ServiceResult<BusinessImageDto>.SuccessResult(imageDto);
        }

        /// <summary>
        /// Delete an image from the cloud and remove it from the business gallery.
        /// </summary>
        public async Task<ServiceResult> DeleteImageAsync(int businessId, int imageId)
        {
            var spec = new BusinessSpecifications
            {
                IncludeImages = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            var image = business.Images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
            {
                _logger.LogWarning("Image {ImageId} not found for business {BusinessId}.", imageId, businessId);
                return ServiceResult.Failure("Image not found.", 404);
            }

            
            var fileName = image.Url.Split('/').Last();
            try
            {
                await _googleCloudStorageService.DeleteFileAsync("business-images", fileName);
                _logger.LogInformation("Image file {FileName} deleted from storage.", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image file {FileName} from cloud.", fileName);
                return ServiceResult.Failure("Failed to delete file from storage.", 500);
            }

            business.Images.Remove(image);

            if (image.IsPrimary && business.Images.Any())
            {
                var newPrimary = business.Images.First();
                newPrimary.IsPrimary = true;

                _logger.LogInformation("Image {ImageId} was primary. Set new primary image to {NewImageId}", image.Id, newPrimary.Id);
            }

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Image {ImageId} deleted for business {BusinessId}.", imageId, businessId);
            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Sets business image as the primary image.
        /// </summary>
        public async Task<ServiceResult> SetPrimaryImageAsync(int businessId, int imageId)
        {
            var spec = new BusinessSpecifications
            {
                IncludeImages = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            var image = business.Images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
            {
                _logger.LogWarning("Image {ImageId} not found for business {BusinessId}.", imageId, businessId);
                return ServiceResult.Failure("Image not found.", 404);
            }

            if (image.IsPrimary)
            {
                _logger.LogInformation("Image {ImageId} is already primary for business {BusinessId}.", imageId, businessId);
                return ServiceResult.SuccessResult();
            }

            foreach (var img in business.Images)
                img.IsPrimary = false;

            image.IsPrimary = true;

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Image {ImageId} set as primary for business {BusinessId}.", imageId, businessId);
            return ServiceResult.SuccessResult();
        }
    }
}
