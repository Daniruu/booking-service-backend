using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IGoogleCloudStorageService googleCloudStorageService,
            ILogger<UserService> logger
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _googleCloudStorageService = googleCloudStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a user by ID and maps it to a UserDto.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>
        /// A ServiceResult containing the UserDto if found; 
        /// otherwise, a failure result with an error message and status code.
        /// </returns>
        public async Task<ServiceResult<UserDto>> GetByIdAsync(int userId)
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID received: {UserId}", userId);
                return ServiceResult<UserDto>.Failure("Invalid user ID.", 400);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return ServiceResult<UserDto>.Failure("User not found.", 404);
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ServiceResult<UserDto>.SuccessResult(userDto);
        }

        /// <summary>
        /// Updates the name and surname of a user based on the provided user ID and data.
        /// Only non-null fields in the DTO will be updated.
        /// </summary>
        /// <param name="userId">The ID of the user to be updated.</param>
        /// <param name="dto">DTO containing fields to be updated.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating the outcome of the operation:
        /// - 204 No Content if successful;
        /// - 400 Bad Request if the user ID is invalid;
        /// - 404 Not Found if the user does not exist.
        /// </returns>
        /// <remarks>
        /// This method does not update other fields such as email, phone, or avatar.
        /// </remarks>
        public async Task<ServiceResult> UpdateUserAsync(int userId, PatchUserDto dto)
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID received: {UserId}", userId);
                return ServiceResult<UserDto>.Failure("Invalid user ID.", 400);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return ServiceResult<UserDto>.Failure("User not found.", 404);
            }

            if (dto.Name != null)
                user.Name = dto.Name;

            if (dto.Surname != null)
                user.Surname = dto.Surname;

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} successfully updated: {Fields}", userId,
                new { UpdatedName = dto.Name != null, UpdatedSurname = dto.Surname != null });

            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Uploads a new avatar for the specified user. If an existing avatar is present, it will be deleted first.
        /// </summary>
        /// <param name="userId">ID of the user for whom the avatar is being uploaded.</param>
        /// <param name="file">The image file to upload (must be jpg/jpeg/png, max 5MB).</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing the URL to the uploaded avatar on success,
        /// or an error message and status code on failure.
        /// </returns>
        public async Task<ServiceResult<string>> UploadAvatarAsync(int userId, IFormFile file)
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID received: {UserId}", userId);
                return ServiceResult<string>.Failure("Invalid user ID.", 400);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return ServiceResult<string>.Failure("User not found.", 404);
            }

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var existingFileName = user.AvatarUrl.Split('/').Last();

                try
                {
                    await _googleCloudStorageService.DeleteFileAsync("avatars", existingFileName);
                    _logger.LogInformation("Deleted previous avatar '{FileName}' for user {UserId}", existingFileName, userId);

                    user.AvatarUrl = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete previous avatar '{FileName}' for user {UserId}", existingFileName, userId);
                }
            }

            var fileUrl = await _googleCloudStorageService.UploadFileAsync("avatars", file);
            _logger.LogInformation("Uploaded new avatar for user {UserId}: {Url}", userId, fileUrl);

            user.AvatarUrl = fileUrl;
            await _userRepository.UpdateAsync(user);

            return ServiceResult<string>.SuccessResult(fileUrl);
        }

        /// <summary>
        /// Deletes the user's avatar from storage and updates the profile.
        /// </summary>
        /// <param name="userId">The ID of the user whose avatar should be deleted.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success or failure.
        /// </returns>
        public async Task<ServiceResult> DeleteAvatarAsync(int userId)
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID received: {UserId}", userId);
                return ServiceResult.Failure("Invalid user ID.", 400);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return ServiceResult.Failure("User not found.", 404);
            }

            if (string.IsNullOrEmpty(user.AvatarUrl))
            {
                _logger.LogWarning("No avatar to delete for user {UserId}", userId);
                return ServiceResult.Failure("No avatar to delete.", 404);
            }

            var fileName = user.AvatarUrl.Split('/').Last();

            try
            {
                await _googleCloudStorageService.DeleteFileAsync("avatars", fileName);
                _logger.LogInformation("Deleted avatar '{FileName}' for user {UserId}", fileName, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete avatar '{FileName}' from storage for user {UserId}", fileName, userId);
                return ServiceResult.Failure("Failed to delete avatar from storage.", 500);
            }

            user.AvatarUrl = null;
            await _userRepository.UpdateAsync(user);

            return ServiceResult.SuccessResult();
        }
    }
}
