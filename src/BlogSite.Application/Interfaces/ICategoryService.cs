using BlogSite.Application.DTOs;

namespace BlogSite.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<CategoryDto?> GetCategoryByNameAsync(string name);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
    Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateDto);
    Task DeleteCategoryAsync(Guid id);
}