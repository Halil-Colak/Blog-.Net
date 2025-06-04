using Blog.Data.Context;
using Blog.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Panel.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly BaseContext _context;
    public ReportController(BaseContext context) { _context = context; }

    #region List
    public IActionResult List()
    {
        List<Report> reports = _context.Reports.OrderByDescending(x => x.CreatedDateTime).ToList();
        return View(reports);
    }
    #endregion

    [Route("/Report/CommentStatusChange/{id}")]
    public IActionResult CommentStatusChange(int id)
    {
        Report? report = _context.Reports.FirstOrDefault(x => x.Id == id);

        if (report == null)
        {
            TempData["Failed"] = "Rapor Bulunamadı";
        }

        report.IsActive = !report.IsActive;

        _context.SaveChanges();

        TempData["Success"] = "Statüs değiştirildi";
        return RedirectToAction("List");
    }
}
