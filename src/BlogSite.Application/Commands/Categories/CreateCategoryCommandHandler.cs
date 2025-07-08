using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Commands.Categories;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Check if category name already exists
        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name);
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Category with name '{request.Name}' already exists.");
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(createdCategory);
    }
}