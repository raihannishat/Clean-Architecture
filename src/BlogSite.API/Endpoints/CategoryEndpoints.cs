using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/categories")
            .WithTags("Categories");

        // GET /api/categories
        group.MapGet("/", GetAllCategories)
            .WithName("GetAllCategories")
            .WithSummary("Get all categories")
            .WithDescription("Returns a list of all categories")
            .Produces<IEnumerable<CategoryDto>>();

        // GET /api/categories/{id}
        group.MapGet("/{id:guid}", GetCategory)
            .WithName("GetCategory")
            .WithSummary("Get a category by ID")
            .WithDescription("Returns a category by its unique identifier")
            .Produces<CategoryDto>()
            .Produces(404);

        // GET /api/categories/name/{name}
        group.MapGet("/name/{name}", GetCategoryByName)
            .WithName("GetCategoryByName")
            .WithSummary("Get a category by name")
            .WithDescription("Returns a category by its name")
            .Produces<CategoryDto>()
            .Produces(404);

        // POST /api/categories
        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new category")
            .WithDescription("Creates a new category with the provided information")
            .Produces<CategoryDto>(201)
            .Produces(400);

        // PUT /api/categories/{id}
        group.MapPut("/{id:guid}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithSummary("Update a category")
            .WithDescription("Updates an existing category with the provided information")
            .Produces<CategoryDto>()
            .Produces(400)
            .Produces(404);

        // DELETE /api/categories/{id}
        group.MapDelete("/{id:guid}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithSummary("Delete a category")
            .WithDescription("Deletes a category by its unique identifier")
            .Produces(204)
            .Produces(400)
            .Produces(404);
    }

    private static async Task<IResult> GetAllCategories(ICategoryService categoryService, ILogger<Program> logger)
    {
        try
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return Results.Ok(categories);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all categories");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetCategory(Guid id, ICategoryService categoryService, ILogger<Program> logger)
    {
        try
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return Results.NotFound($"Category with ID {id} not found");
            }
            return Results.Ok(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting category {CategoryId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetCategoryByName(string name, ICategoryService categoryService, ILogger<Program> logger)
    {
        try
        {
            var category = await categoryService.GetCategoryByNameAsync(name);
            if (category == null)
            {
                return Results.NotFound($"Category with name {name} not found");
            }
            return Results.Ok(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting category by name {Name}", name);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> CreateCategory(CreateCategoryDto createDto, ICategoryService categoryService, ILogger<Program> logger)
    {
        try
        {
            var createdCategory = await categoryService.CreateCategoryAsync(createDto);
            return Results.CreatedAtRoute("GetCategory", new { id = createdCategory.Id }, createdCategory);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation while creating category");
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating category");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> UpdateCategory(Guid id, UpdateCategoryDto updateDto, ICategoryService categoryService, ILogger<Program> logger)
    {
        try
        {
            var updatedCategory = await categoryService.UpdateCategoryAsync(id, updateDto);
            return Results.Ok(updatedCategory);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Category {CategoryId} not found for update", id);
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation while updating category {CategoryId}", id);
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating category {CategoryId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> DeleteCategory(Guid id, ICategoryService categoryService, ILogger<Program> logger)
    {
        try
        {
            await categoryService.DeleteCategoryAsync(id);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Category {CategoryId} not found for deletion", id);
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation while deleting category {CategoryId}", id);
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting category {CategoryId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }
}