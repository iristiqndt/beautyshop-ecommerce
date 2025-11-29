using ECommerce.Domain.Interfaces;
using ECommerce.Application.DTOs.Category;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDtos = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Slug = c.Slug,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
            return Ok(categoryDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        try
        {
            var category = (await _unitOfWork.Categories.FindAsync(c => c.Slug == slug)).FirstOrDefault();
            if (category == null)
                return NotFound();
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDTO dto)
    {
        try
        {
            var category = new Domain.Entities.Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Slug = dto.Slug,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryDto = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                Slug = category.Slug,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDTO dto)
    {
        try
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existingCategory == null)
                return NotFound();

            existingCategory.Name = dto.Name;
            existingCategory.Description = dto.Description;
            existingCategory.ImageUrl = dto.ImageUrl;
            existingCategory.Slug = dto.Slug;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Categories.UpdateAsync(existingCategory);
            await _unitOfWork.SaveChangesAsync();

            var categoryDto = new CategoryDTO
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                ImageUrl = existingCategory.ImageUrl,
                Slug = existingCategory.Slug,
                CreatedAt = existingCategory.CreatedAt,
                UpdatedAt = existingCategory.UpdatedAt
            };
            
            return Ok(categoryDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            
            return Ok(new { message = "Xóa danh mục thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
