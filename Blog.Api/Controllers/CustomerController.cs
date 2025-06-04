using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : Controller
{

    private readonly BaseContext _context;

    public CustomerController(BaseContext context)
    {
        _context = context;
    }
    [HttpGet("GetBySlug")]
    public IActionResult GetBySlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Ok(new ServiceResponse { Message = "Slug boş olamaz", Status = false });

        var customer = _context.Customers
            .Where(p => p.Slug == slug)
            .Select(p => new
            {
                p.Name,
                p.SurnName,
                p.Email,
                p.Slug,
                p.ProfilPhoto,
                p.CreatedDateTime
            })
            .FirstOrDefault();
        if (customer == null)
            return Ok(new ServiceResponse { Message = "Kullanıcı bulunamadı", Status = false });

        return Ok(new ServiceResponse { Data = customer });
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update([FromForm] UpdateDto customer)
    {
        if (customer == null)
            return Ok(new ServiceResponse { Message = "Kullanıcı verisi alınamadı", Status = false });

        // Kullanıcıyı email ile bul (burada güncellemek isteyen kişi bulunuyor)
        Customer? targetCustomer = _context.Customers.FirstOrDefault(x => x.Email == customer.Email);
        if (targetCustomer == null)
            return Ok(new ServiceResponse { Message = "Kullanıcı bulunamadı", Status = false });

        // Yeni slug oluştur
        string newSlug = SeoUrl.Url(customer.Name + " " + customer.SurName);

        // Slug başka biriyle çakışıyor mu?
        bool slugExists = _context.Customers.Any(x => x.Slug == newSlug && x.Id != targetCustomer.Id);
        if (slugExists)
            return Ok(new ServiceResponse { Status = false, Message = "Bu ada sahip başka bir kullanıcı bulunmakta" });

        // Email başka biriyle çakışıyor mu?
        bool emailExists = _context.Customers.Any(x => x.Email == customer.Email && x.Id != targetCustomer.Id);
        if (emailExists)
            return Ok(new ServiceResponse { Status = false, Message = "Bu email başka bir kullanıcıya ait" });

        if (customer.Photo != null)
        {
            targetCustomer.ProfilPhoto = ImageSaveApi.SaveImagetoFolder(customer.Photo);
        }

        targetCustomer.Name = customer.Name == "null" ? null : customer.Name;
        targetCustomer.SurnName = customer.SurName == "null" ? null : customer.SurName;
        targetCustomer.Slug = newSlug;

        await _context.SaveChangesAsync();

        return Ok(new ServiceResponse { Status = true, Message = "Güncelleme başarılı" });
    }

    public class UpdateDto
    {
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string Email { get; set; }
        public IFormFile? Photo { get; set; }
    }


    [HttpGet("CustomerLikeBlog")]
    public IActionResult CustomerLikeBlog([FromQuery] string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Ok(new ServiceResponse { Status = false, Message = "Slug boş olamaz." });

        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new ServiceResponse { Status = false, Message = "Geçersiz sayfa veya sayfa boyutu." });
        }

        Customer? customer = _context.Customers.FirstOrDefault(x => x.Slug == slug && x.IsActive);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı." });

        var customerLikeBlog = _context.BlogLikes
        .Where(x => x.CustomerId == customer.Id && x.IsActive)
        .OrderByDescending(x => x.CreatedDateTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(bl => new
        {
            Title = bl.Blog.Title,
            Slug = bl.Blog.Slug,
            Description = bl.Blog.Description,
            ImageUrl = bl.Blog.ImageUrl,
            IsPopular = bl.Blog.IsPopular,
            CreatedDateTime = bl.Blog.CreatedDateTime,
            BlogLikes = bl.Blog.BlogLikes.Count(),
            Author = new { bl.Blog.User.Name, bl.Blog.User.Surname, bl.Blog.User.ProfileImage },
        })
        .ToList();

        return Ok(new ServiceResponse { Data = customerLikeBlog });
    }


    [HttpGet("CustomerCommentBlog")]
    public IActionResult CustomerCommentBlog([FromQuery] string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Ok(new ServiceResponse { Status = false, Message = "Slug boş olamaz." });

        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new ServiceResponse { Status = false, Message = "Geçersiz sayfa veya sayfa boyutu." });
        }

        Customer? customer = _context.Customers.FirstOrDefault(x => x.Slug == slug && x.IsActive);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı." });

        var groupedComments = _context.Comments
            .Where(x => x.CustomerId == customer.Id && x.IsActive)
            .GroupBy(x => new { x.CustomerId, x.BlogId }) // Aynı müşteri ve blog için grupla
            .OrderByDescending(group => group.Max(x => x.CreatedDateTime))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(group => new
            {
                Comments = group.Select(g => new
                {
                    Content = g.Content,
                    CreatedDateTime = g.CreatedDateTime
                }).ToList(),
                Title = group.First().Blog.Title,
                Slug = group.First().Blog.Slug,
                ImageUrl = group.First().Blog.ImageUrl,
                Description = group.First().Blog.Description,
                IsPopular = group.First().Blog.IsPopular,
                CreatedDateTime = group.First().Blog.CreatedDateTime,
                BlogLikes = group.First().Blog.BlogLikes.Count(),
                Author = new { group.First().Blog.User.Name, group.First().Blog.User.Surname, group.First().Blog.User.ProfileImage },
                Customer = new
                {
                    group.First().Customer.Name,
                    group.First().Customer.SurnName,
                    group.First().Customer.ProfilPhoto,
                }

            })
            .ToList();

        return Ok(new ServiceResponse { Data = groupedComments });
    }



}
