using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Panel.Controllers;

[Authorize]

public class CategoryController : Controller
{

    private readonly BaseContext _context;
    public CategoryController(BaseContext context) { _context = context; }

    #region List
    public IActionResult List()
    {
        List<Category> categories = _context.Categories.OrderByDescending(x => x.CreatedDateTime).ToList();
        return View(categories);
    }
    #endregion

    #region Add
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(Category category)
    {
        string slug = SeoUrl.Url(category.Name);
        category.Slug = slug;

        Category? controlName = _context.Categories.FirstOrDefault(c => c.Slug == slug);
        if (controlName == null)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            TempData["Success"] = "Kategori eklendi";
            return RedirectToAction("List");
        }
        TempData["Failed"] = "Veri tabanında aynı kategori var zaten avell";
        return RedirectToAction("List");
    }
    #endregion

    #region Update

    [Route("/Category/Update/{id}")]
    public IActionResult Update(int id)
    {
        Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
        {
            TempData["Failed"] = "Kategori bulunamadı(ID)";
            return RedirectToAction("List");
        }
        return View(category);
    }

    [HttpPost, Route("/Category/Update/{id}")]
    public IActionResult Update(int id, Category category)
    {

        string slug = SeoUrl.Url(category.Name);

        bool isSlugExists = _context.Categories
            .AsNoTracking()
            .Any(c => c.Slug == slug && c.Id != id);

        if (!isSlugExists)
        {
            category.Slug = slug;
            _context.Categories.Update(category);
            _context.SaveChanges();
            TempData["Success"] = "Kategori güncellendi";
            return RedirectToAction("List");
        }

        TempData["Failed"] = "Bu isimde başka bir kategori zaten var!";
        return RedirectToAction("List");
    }

    #endregion

    #region Delete

    //[Route("/Category/Delete/{id}")]
    //public IActionResult Delete(int id) {
    //	Category? category = _context.Categories.FirstOrDefault(x => x.Id == id);
    //	if (category == null) {
    //		TempData["Failed"] = "Kategori bulunamadı(ID)";
    //		return RedirectToAction("List");
    //	}
    //	_context.Categories.Remove(category);
    //	_context.SaveChanges();
    //	TempData["Success"] = "Kategori silindi";
    //	return RedirectToAction("List");
    //}
    #endregion
}
