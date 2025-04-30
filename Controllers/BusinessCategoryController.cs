using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [AllowAnonymous]
    public class BusinessCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<BusinessCategoryController> _logger;

        public BusinessCategoryController(ICategoryService categoryService, ILogger<BusinessCategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessCategories()
        {
            _logger.LogInformation("Retrieving business categories");
            var result = await _categoryService.GetAllCategories();

            if (!result.Success)
            {
                _logger.LogWarning("Failed to get business categories. Reason: {Reason}", result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Successfully retrieved {Count} categories", result.Data.Count());
            return Ok(new { categories = result.Data });
        }
    }
}
