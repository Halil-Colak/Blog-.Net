using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Data.Entity.Junction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogController : Controller
{
    private readonly BaseContext _context;

    public BlogController(BaseContext context)
    {
        _context = context;
    }

    [HttpGet("GetBlogsPage")]
    public async Task<IActionResult> GetBlogsPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new ServiceResponse { Status = false, Message = "Geçersiz sayfa veya sayfa boyutu." });
        }

        var blogs = await _context.Blogs
            .Where(p => p.IsActive)
            .Include(p => p.User)
            .Include(p => p.BlogLikes)
            .OrderByDescending(p => p.CreatedDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Title,
                p.Slug,
                p.Description,
                p.ImageUrl,
                p.CreatedDateTime,
                BlogLikes = p.BlogLikes.Count(),
                Author = new { p.User.Name, p.User.Surname, p.User.ProfileImage },
            })
            .ToListAsync();

        return Ok(new ServiceResponse { Data = blogs });
    }

    [HttpGet("GetBlogBySlug")]
    public async Task<IActionResult> GetBlogBySlug([FromQuery] string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return Ok(new ServiceResponse { Status = false, Message = "Slug boş olamaz." });
        }


        var blog = await _context.Blogs
            .Where(p => p.IsActive)
            .Include(p => p.User)
            .Include(p => p.BlogLikes)
            .Include(p => p.BlogCategorys)
            .ThenInclude(pc => pc.Category)
            .Where(p => p.Slug == slug)
            .Select(p => new
            {
                p.Title,
                p.Slug,
                p.Description,
                p.Content,
                p.ImageUrl,
                p.CreatedDateTime,
                BlogLikes = p.BlogLikes.Count(),
                Author = new { p.User.Name, p.User.Surname, p.User.ProfileImage },
                Categories = p.BlogCategorys
                    .Where(pc => pc.Category.IsActive)
                    .Select(pc => new { pc.Category.Slug, pc.Category.Name })
            })
            .FirstOrDefaultAsync();

        if (blog == null)
        {
            return Ok(new ServiceResponse { Status = false, Message = "Belirtilen slug ile eşleşen blog bulunamadı." });
        }

        return Ok(new ServiceResponse { Data = blog });
    }


    [HttpGet("GetBlogsByCategory")]
    public async Task<IActionResult> GetBlogsByCategory([FromQuery] string categorySlug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(categorySlug))
        {
            return Ok(new ServiceResponse { Status = false, Message = "Geçersiz kategori slug değeri.", Data = new List<object>() });
        }

        if (page < 1 || pageSize < 1)
        {
            return Ok(new ServiceResponse { Status = false, Message = "Geçersiz sayfa veya sayfa boyutu.", Data = new List<object>() });
        }

        var blogs = await _context.Blogs
            .Where(p => p.IsActive)
            .Where(p => p.BlogCategorys
                .Any(pc => pc.Category.Slug == categorySlug && pc.Category.IsActive))
            .Include(p => p.User)
            .Include(p => p.BlogLikes)
            .OrderByDescending(p => p.CreatedDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Title,
                p.Slug,
                p.Description,
                p.ImageUrl,
                p.CreatedDateTime,
                BlogLikes = p.BlogLikes.Count(),
                Author = new { p.User.Name, p.User.Surname, p.User.ProfileImage },
            })
            .ToListAsync();

        if (!blogs.Any())
        {
            return Ok(new ServiceResponse { Status = false, Message = "Bu kategoriye ait aktif post bulunamadı.", Data = new List<object>() });
        }

        return Ok(new ServiceResponse { Data = blogs });
    }


    [HttpGet("GetRandomBlog")]
    public async Task<IActionResult> GetRandomBlog()
    {
        var blog = await _context.Blogs
            .Where(p => p.IsActive)
            .Include(p => p.User)
            .Include(p => p.BlogLikes)
            .Include(p => p.BlogCategorys)
            .ThenInclude(pc => pc.Category)
            .OrderBy(p => Guid.NewGuid())
            .Select(p => new
            {
                p.Title,
                p.Slug,
                p.Description,
                p.Content,
                p.ImageUrl,
                p.CreatedDateTime,
                BlogLikes = p.BlogLikes.Count(),
                Author = new { p.User.Name, p.User.Surname, p.User.ProfileImage },
                Categories = p.BlogCategorys
                    .Where(pc => pc.Category.IsActive)
                    .Select(pc => new { pc.Category.Slug, pc.Category.Name })
            })
            .FirstOrDefaultAsync();

        if (blog == null)
        {
            return Ok(new ServiceResponse { Status = false, Message = "Hiçbir aktif post bulunamadı." });
        }

        return Ok(new ServiceResponse { Data = blog });
    }


    [HttpGet("GetPopularBlogsByCategory")]
    public async Task<IActionResult> GetPopularBlogsByCategory([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? slug = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return Ok(new ServiceResponse { Status = false, Message = "Geçersiz sayfa veya sayfa boyutu." });
        }

        IQueryable<Data.Entity.Blog> query = _context.Blogs
            .Where(p => p.IsActive && p.IsPopular)
            .Include(p => p.User)
            .Include(p => p.BlogLikes)
            .OrderByDescending(p => p.CreatedDateTime)
            .AsQueryable();

        if (!string.IsNullOrEmpty(slug))
        {
            query = query.Where(p => p.BlogCategorys.Any(pc => pc.Category.IsActive && pc.Category.Slug == slug));
        }

        var blogs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Title,
                p.Slug,
                p.Description,
                p.ImageUrl,
                p.CreatedDateTime,
                BlogLikes = p.BlogLikes.Count(),
                Author = new { p.User.Name, p.User.Surname, p.User.ProfileImage },
            })
            .ToListAsync();

        if (!blogs.Any())
        {
            return Ok(new ServiceResponse { Status = false, Message = "Hiçbir gönderi bulunamadı." });
        }

        return Ok(new ServiceResponse { Status = true, Data = blogs });
    }


    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string? text, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Ok(new ServiceResponse { Status = false, Message = "Arama boş olamaz." });
        }

        IQueryable<Data.Entity.Blog> query = _context.Blogs
            .Where(p => p.IsActive && p.Title.Contains(text))
            .Include(p => p.User)
            .Include(p => p.BlogLikes)

            .AsQueryable();

        int totalItems = await query.CountAsync();
        var blogs = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Title,
                p.Slug,
                p.Description,
                p.ImageUrl,
                p.CreatedDateTime,
                BlogLikes = p.BlogLikes.Count(),
                Author = new { p.User.Name, p.User.Surname, p.User.ProfileImage },

            })
            .ToListAsync();

        if (!blogs.Any())
        {
            return Ok(new ServiceResponse { Status = false, Message = "Belirtilen blog bulunamadı." });
        }

        return Ok(new ServiceResponse { Status = true, Data = blogs, });
    }


    [HttpGet("BlogLike")]
    public async Task<IActionResult> BlogLike([FromQuery] string slug, [FromQuery] string email)
    {
        if (slug == "" || email == "") return Ok(new ServiceResponse { Status = false, Message = "Bilgiler eksik" });
        Data.Entity.Blog? blog = _context.Blogs.FirstOrDefault(p => p.Slug == slug);
        if (blog == null) return Ok(new ServiceResponse { Status = false, Message = "Blog bulunamadı" });
        Customer? customer = _context.Customers.FirstOrDefault(p => p.Email == email);
        if (customer == null) return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı" });

        BlogLike? like = _context.BlogLikes.FirstOrDefault(x => x.CustomerId == customer.Id && x.BlogId == blog.Id);
        if (like == null)
        {
            BlogLike liked = new()
            {
                CustomerId = customer.Id,
                BlogId = blog.Id,
            };
            _context.BlogLikes.Add(liked);
            _context.SaveChanges();
            return Ok(new ServiceResponse { Status = true, Data = true });
        }
        else
        {
            _context.BlogLikes.Remove(like);
            _context.SaveChanges();
            return Ok(new ServiceResponse { Status = true, Data = false });
        }


    }

    [HttpGet("IsBlogLiked")]
    public async Task<IActionResult> IsBlogLiked([FromQuery] string slug, [FromQuery] string email)
    {
        if (slug == "" || email == "") return Ok(new ServiceResponse { Status = false, Message = "Bilgiler eksik" });
        Data.Entity.Blog? blog = _context.Blogs.FirstOrDefault(p => p.Slug == slug);
        if (blog == null) return Ok(new ServiceResponse { Status = false, Message = "Blog bulunamadı" });
        Customer? customer = _context.Customers.FirstOrDefault(p => p.Email == email);
        if (customer == null) return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı" });

        bool like = _context.BlogLikes.Any(x => x.CustomerId == customer.Id && x.BlogId == blog.Id);

        return Ok(new ServiceResponse { Status = true, Data = like });

    }



}
