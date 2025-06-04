using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Panel.Controllers;

[Authorize]

public class UserController : Controller
{

    private readonly BaseContext _context;
    public UserController(BaseContext context) { _context = context; }
    public IActionResult List()
    {
        List<User> users = _context.Users.OrderByDescending(x => x.CreatedDateTime).ToList();
        return View(users);
    }
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(User user, IFormFile CoverProfileImage)
    {

        user.ProfileImage = ImageSave.SaveImagetoFolder(CoverProfileImage);
        user.PaswortHas = BCryptHash.BCryptHashPassword(user.PaswortHas);
        _context.Users.Add(user);
        _context.SaveChanges();

        TempData["Success"] = "Kullanıcı Eklendi";
        return RedirectToAction("List");
    }

    [Route("/Post/Update/{id}")]
    public IActionResult Update(int id)
    {
        User? user = _context.Users.FirstOrDefault(x => x.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    [Route("/Post/Update/{id}")]
    public IActionResult Update(User user, IFormFile CoverProfileImage)
    {
        User? existingUser = _context.Users.FirstOrDefault(x => x.Id == user.Id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // İsim, soyisim, email güncelleme
        existingUser.Name = user.Name;
        existingUser.Surname = user.Surname;
        existingUser.Email = user.Email;

        // Şifre güncelleme (boş değilse)
        if (!string.IsNullOrEmpty(user.PaswortHas))
        {
            existingUser.PaswortHas = BCryptHash.BCryptHashPassword(user.PaswortHas);
        }

        // Fotoğraf güncelleme (null değilse)
        if (CoverProfileImage != null)
        {
            existingUser.ProfileImage = ImageSave.SaveImagetoFolder(CoverProfileImage);
        }

        _context.SaveChanges();

        TempData["Success"] = "Kullanıcı Güncellendi";
        return RedirectToAction("List");
    }


}
