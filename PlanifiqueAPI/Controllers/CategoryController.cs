using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using PlanifiqueAPI.Core.Interfaces;
using System.Security.Claims;

namespace PlanifiqueAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var categories = await _categoryService.GetCategoriesAsync(userId);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);

            if (category == null) return NotFound("Categoria não encontrada.");

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var category = await _categoryService.CreateCategoryAsync(categoryDto, userId);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto categoryDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _categoryService.UpdateCategoryAsync(id, categoryDto, userId);

            if (!success) return NotFound("Categoria não encontrada ou não pertence ao usuário.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _categoryService.DeleteCategoryAsync(id, userId);

            if (!success) return NotFound("Categoria não encontrada ou não pertence ao usuário.");

            return NoContent();
        }
    }
}
