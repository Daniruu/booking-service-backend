using BookingService.DTOs;
using BookingService.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by ID and maps it to a UserDto.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>
        /// A ServiceResult containing the UserDto if found; 
        /// otherwise, a failure result with an error message and status code.
        /// </returns>
        Task<ServiceResult<UserDto>> GetByIdAsync(int userId);

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
        Task<ServiceResult> UpdateUserAsync(int userId, PatchUserDto dto);

        // <summary>
        /// Uploads a new avatar for the specified user. If an existing avatar is present, it will be deleted first.
        /// </summary>
        /// <param name="userId">ID of the user for whom the avatar is being uploaded.</param>
        /// <param name="file">The image file to upload (must be jpg/jpeg/png, max 5MB).</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing the URL to the uploaded avatar on success,
        /// or an error message and status code on failure.
        /// </returns>
        Task<ServiceResult<string>> UploadAvatarAsync(int userId, IFormFile file);

        /// <summary>
        /// Deletes the user's avatar from storage and updates the profile.
        /// </summary>
        /// <param name="userId">The ID of the user whose avatar should be deleted.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult> DeleteAvatarAsync(int userId);
    }
}
