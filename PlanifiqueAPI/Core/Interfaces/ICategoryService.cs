using PlanifiqueAPI.Application.DTOs;

namespace PlanifiqueAPI.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<ReadCategoryDto>> GetCategoriesAsync(string userId);
        Task<ReadCategoryDto> GetCategoryByIdAsync(int id, string userId);
        Task<ReadCategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, string userId);
        Task<bool> UpdateCategoryAsync(int id, CreateCategoryDto categoryDto, string userId);
        Task<bool> DeleteCategoryAsync(int id, string userId);
    }
}
