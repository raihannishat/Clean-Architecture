using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto?> GetCategoryByNameAsync(string name)
    {
        var category = await _repository.GetByNameAsync(name);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
    {
        // Check if category name already exists
        var existingCategory = await _repository.GetByNameAsync(createDto.Name);
        if (existingCategory != null)
        {
            throw new ArgumentException($"Category with name '{createDto.Name}' already exists.");
        }

        var category = _mapper.Map<Category>(createDto);
        var createdCategory = await _repository.AddAsync(category);
        return _mapper.Map<CategoryDto>(createdCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateDto)
    {
        var existingCategory = await _repository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        // Check if new name conflicts with existing category
        if (existingCategory.Name != updateDto.Name)
        {
            var conflictingCategory = await _repository.GetByNameAsync(updateDto.Name);
            if (conflictingCategory != null)
            {
                throw new ArgumentException($"Category with name '{updateDto.Name}' already exists.");
            }
        }

        _mapper.Map(updateDto, existingCategory);
        var updatedCategory = await _repository.UpdateAsync(existingCategory);
        return _mapper.Map<CategoryDto>(updatedCategory);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        if (category.BlogPosts.Any())
        {
            throw new InvalidOperationException("Cannot delete category that has blog posts. Delete or reassign posts first.");
        }

        await _repository.DeleteAsync(id);
    }
}