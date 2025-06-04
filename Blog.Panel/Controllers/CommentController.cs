using Blog.Data.Context;
using Blog.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Panel.Controllers;
public class CommentController : Controller
{
    private readonly BaseContext _context;
    public CommentController(BaseContext context) { _context = context; }
    public IActionResult List()
    {
        List<Comment> comment = _context.Comments.
            OrderByDescending(x => x.CreatedDateTime)
            .Include(x => x.Customer)
            .ToList();
        return View(comment);
    }
    [Route("/Comment/CommentStatusChange/{id}")]
    public IActionResult CommentStatusChange(int id)
    {
        Comment? comment = _context.Comments.FirstOrDefault(x => x.Id == id);

        if (comment == null)
        {
            TempData["Failed"] = "Yorum Bulunamadı";
        }

        comment.IsActive = !comment.IsActive;

        _context.SaveChanges();

        TempData["Success"] = "Statüs değiştirildi";
        return RedirectToAction("List");
    }
}
