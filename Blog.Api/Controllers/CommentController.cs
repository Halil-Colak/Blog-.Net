using Blog.Data.Context;
using Blog.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : Controller
{

    private readonly BaseContext _context;

    public CommentController(BaseContext context)
    {
        _context = context;
    }


    [HttpPost("Add")]
    public IActionResult Add([FromBody] CreateCommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            return Ok(new ServiceResponse { Status = false, Message = "Beklenmedik hata" });

        // Blog var mı kontrolü
        Data.Entity.Blog? blog = _context.Blogs.FirstOrDefault(b => b.Slug == dto.BlogSlug);
        if (blog == null)
            return Ok(new ServiceResponse { Status = false, Message = "Blog bulunamadı." });

        Customer? customer = _context.Customers.FirstOrDefault(c => c.Slug == dto.CustomerSlug);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı." });

        Data.Entity.Comment comment = new()
        {
            Content = dto.Content,
            BlogId = blog.Id,
            CustomerId = customer.Id,
            IsActive = false
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        return Ok(new ServiceResponse { Message = "Yorum başarıyla eklend" });
    }
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public string BlogSlug { get; set; }
        public string CustomerSlug { get; set; }
    }

    [HttpGet("TaretBlogComment")]
    public IActionResult TaretBlogComment([FromQuery] string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Ok(new ServiceResponse { Status = false, Message = "Slug boş olamaz." });

        if (page < 1 || pageSize < 1)
        {
            return Ok(new ServiceResponse { Status = false, Message = "Geçersiz sayfa veya sayfa boyutu.", Data = new List<object>() });
        }
        Data.Entity.Blog? blog = _context.Blogs.FirstOrDefault(b => b.Slug == slug);
        if (blog == null)
            return Ok(new ServiceResponse { Status = false, Message = "Blog bulunamadı." });

        var comments = _context.Comments
                        .Where(x => x.BlogId == blog.Id && x.IsActive)
                        .OrderByDescending(p => p.CreatedDateTime)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(x => new
                        {
                            x.Id,
                            x.Content,
                            x.CreatedDateTime,
                            Customer = x.Customer == null ? null : new
                            {
                                x.Customer.Name,
                                x.Customer.SurnName,
                                x.Customer.Slug,
                                x.Customer.ProfilPhoto,
                            }
                        })
        .ToList();


        return Ok(new ServiceResponse { Data = comments });
    }

    [HttpPost("Delete")]
    public IActionResult Delete([FromForm] DeleteDto deleteDto)
    {
        if (deleteDto.CommentId < 0 || deleteDto.CustomerSlug == "")
            return Ok(new ServiceResponse { Status = false, Message = "Bilgiler yanlış tekrar deneyin." });

        Customer? customer = _context.Customers.FirstOrDefault(x => x.Slug == deleteDto.CustomerSlug);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı." });


        Data.Entity.Comment? comment = _context.Comments.FirstOrDefault(x => x.Id == deleteDto.CommentId && x.CustomerId == customer.Id);
        if (comment == null)
            return Ok(new ServiceResponse { Status = false, Message = "Yorum bulunamadı." });

        _context.Comments.Remove(comment);
        _context.SaveChanges();
        return Ok(new ServiceResponse { Message = "Yorumunuz başarıyla silindi." });
    }

    public class DeleteDto
    {
        public int CommentId { get; set; }
        public string CustomerSlug { get; set; }
    }




}
