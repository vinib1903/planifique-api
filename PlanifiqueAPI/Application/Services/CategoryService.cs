using Microsoft.EntityFrameworkCore;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using PlanifiqueAPI.Core.Interfaces;
using PlanifiqueAPI.Infraestructure.Data;

namespace PlanifiqueAPI.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReadCategoryDto>> GetCategoriesAsync(string userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new ReadCategoryDto { Id = c.Id, Nome = c.Nome, Color = c.Color })
                .ToListAsync();
        }

        public async Task<ReadCategoryDto> GetCategoryByIdAsync(int id, string userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) return null;

            return new ReadCategoryDto { Id = category.Id, Nome = category.Nome, Color = category.Color };
        }

        public async Task<ReadCategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, string userId)
        {
            var category = new Category
            {
                Nome = categoryDto.Nome,
                UserId = userId,
                Color = categoryDto.Color
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new ReadCategoryDto { Id = category.Id, Nome = category.Nome };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CreateCategoryDto categoryDto, string userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) return false;

            category.Nome = categoryDto.Nome;
            category.Color = categoryDto.Color;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id, string userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

