using Blog.Data.Context;
using Blog.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class ReportController : Controller
{
    private readonly BaseContext _context;

    public ReportController(BaseContext context)
    {
        _context = context;
    }
    [HttpPost("Add")]
    public IActionResult Add([FromForm] string content)
    {
        if (content == null || content == "")
            return Ok(new ServiceResponse { Message = "Rapor verisi alınamadı", Status = false });

        Report report = new() { Content = content, IsActive = false };

        _context.Reports.Add(report);
        _context.SaveChanges();
        return Ok(new ServiceResponse { Message = "Bildiri alındı. Teşekkürler" });
    }
}
