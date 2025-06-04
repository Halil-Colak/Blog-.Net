using Blog.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : Controller {
	private readonly BaseContext _context;

	public CategoryController(BaseContext context) {
		_context = context;
	}

	[HttpGet("CategoryGetAll")]
	public async Task<IActionResult> CategoryGetAll() {
		var categories = await _context.Categories
			.Where(x => x.IsActive)
			.Select(x => new { x.Slug, x.Name })
			.ToListAsync();

		if (categories == null || !categories.Any()) {
			return NotFound("Hiçbir aktif kategori bulunamadı.");
		}

		return Ok(categories);
	}
	[HttpGet("CategoryBySlug")]
	public async Task<IActionResult> CategoryBySlug(string slug) {
		var category = await _context.Categories
			.Where(x => x.IsActive && x.Slug == slug)
			.Select(x => new { x.Slug, x.Name })
			.FirstOrDefaultAsync();

		if (category == null) {
			return NotFound("Hiçbir aktif kategori bulunamadı.");
		}
		return Ok(category);
	}

}