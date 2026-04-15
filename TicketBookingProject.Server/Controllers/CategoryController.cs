using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _cateService;
        public CategoryController(ICategoryService cateService) {
            _cateService = cateService;
        }

        [HttpGet("names")]
        public async Task<IActionResult> ListCategoryNames(string? req)
        {
            var cateNames = await _cateService.ListCategoryNames(req);

            return Ok(ApiResponse<List<string>>.Ok(cateNames));
        }

        [HttpGet]
        public async Task<IActionResult> ListCategory()
        {
            var cate = await _cateService.ListCategory();
            return Ok(ApiResponse<List<CategoryResponse>>.Ok(cate));
        }
    }
}
