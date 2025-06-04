using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Data.Entity.Junction;
using Blog.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Panel.Controllers;

[Authorize]

public class BlogController : Controller
{
    private readonly BaseContext _context;
    public BlogController(BaseContext context) { _context = context; }
    public IActionResult List()
    {
        List<Data.Entity.Blog> blog = _context.Blogs.OrderByDescending(x => x.CreatedDateTime).ToList();
        return View(blog);
    }

    public IActionResult Add()
    {
        List<Category> category = _context.Categories.Where(x => x.IsActive == true).ToList();
        return View(category);
    }
    [HttpPost]
    public IActionResult Add(Data.Entity.Blog model, IFormFile Image, List<int> BlogCategorys)
    {

        model.Slug = SeoUrl.Url(model.Title);

        bool blogControll = _context.Blogs.Any(x => x.Slug == model.Slug);
        if (blogControll)
        {
            TempData["Failed"] = "Aynı adda blog var la";

            return RedirectToAction("List");
        }
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            model.UserId = userId;
        }
        else
        {
            TempData["Failed"] = "ClaimTypes bulunamdı hacıı";
            return RedirectToAction("List");

        }

        model.ImageUrl = ImageSave.SaveImagetoFolder(Image);
        model.Slug = SeoUrl.Url(model.Title);

        _context.Blogs.Add(model);
        _context.SaveChanges();

        if (BlogCategorys != null && BlogCategorys.Any())
        {
            foreach (int blogCategory in BlogCategorys)
            {
                BlogCategory newBlogCategory = new()
                {
                    BlogId = model.Id, // Yeni eklenen Post'un ID'si
                    CategoryId = blogCategory,// Kullanıcıdan gelen kategori ID
                    IsActive = true
                };

                _context.BlogCategorys.Add(newBlogCategory);
            }

            _context.SaveChanges(); // PostCategory kayıtlarını kaydet
        }
        TempData["Success"] = "Blog Eklendi";
        return RedirectToAction("List");

    }


    [Route("/Blog/Update/{id}")]
    public IActionResult Update(int id)
    {
        List<Category> category = _context.Categories.Where(x => x.IsActive == true).ToList();

        ViewBag.Category = category;

        Data.Entity.Blog? blog = _context.Blogs.FirstOrDefault(x => x.Id == id);

        if (blog == null)
        {
            TempData["Failed"] = "Hatalı ID";
            return RedirectToAction("List");
        }

        List<int> categorySelected = _context.BlogCategorys
            .Where(x => x.BlogId == blog.Id)
            .Select(x => x.CategoryId)
            .ToList();

        ViewBag.CategorySelected = categorySelected;

        return View(blog);
    }


    [HttpPost, Route("/Blog/Update/{id}")]
    public IActionResult Update(int id, Data.Entity.Blog model, IFormFile Image, List<int> BlogCategorys)
    {
        Data.Entity.Blog? existingBlog = _context.Blogs.FirstOrDefault(x => x.Id == id);

        if (existingBlog == null)
        {
            TempData["Failed"] = "Blog bulunamadı.";
            return RedirectToAction("List");
        }

        string newSlug = SeoUrl.Url(model.Title);

        if (existingBlog.Slug != newSlug)
        {
            bool blogControll = _context.Blogs.Any(x => x.Slug == newSlug && x.Id != id);
            if (blogControll)
            {
                TempData["Failed"] = "Aynı adda blog var la";
                return RedirectToAction("List");
            }
            existingBlog.Slug = newSlug;
        }

        existingBlog.Title = model.Title;
        existingBlog.Content = model.Content;
        existingBlog.IsActive = model.IsActive;
        existingBlog.IsPopular = model.IsPopular;
        existingBlog.Description = model.Description;

        if (Image != null)
        {
            existingBlog.ImageUrl = ImageSave.SaveImagetoFolder(Image);
        }

        _context.SaveChanges();

        List<BlogCategory> existingCategories = _context.BlogCategorys.Where(x => x.BlogId == id).ToList();
        _context.BlogCategorys.RemoveRange(existingCategories);

        if (BlogCategorys != null && BlogCategorys.Any())
        {
            foreach (int categoryId in BlogCategorys)
            {
                BlogCategory newBlogCategory = new()
                {
                    BlogId = id,
                    CategoryId = categoryId,
                    IsActive = true
                };
                _context.BlogCategorys.Add(newBlogCategory);
            }
        }

        _context.SaveChanges();

        TempData["Success"] = "Blog Güncellendi";
        return RedirectToAction("List");
    }

}
