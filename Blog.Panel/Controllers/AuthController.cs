using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Panel.Controllers;
public class AuthController : Controller {

	private readonly BaseContext _context;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public AuthController(BaseContext context, IHttpContextAccessor httpContextAccessor) {
		_context = context;
		_httpContextAccessor = httpContextAccessor;


	}

	public IActionResult Login() {
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Login(User user) {
		if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PaswortHas)) {
			TempData["Failed"] = "Lütfen tüm alanları doldurun.";
			return RedirectToAction("Login");
		}

		User userResponse = _context.Users.FirstOrDefault(x => x.Email == user.Email);

		if (userResponse == null) {
			TempData["Failed"] = "Kayıtlı kullanıcı bulunamadı.";
			return RedirectToAction("Login");
		}

		// Şifreyi doğrula
		bool isPasswordValid = BCryptHash.VerifyPassword(user.PaswortHas, userResponse.PaswortHas);

		if (!isPasswordValid) {
			TempData["Failed"] = "Geçersiz şifre.";
			return RedirectToAction("Login");
		}

		List<Claim> claims = new()
		{
	new Claim(ClaimTypes.Email, userResponse.Email),
	new Claim(ClaimTypes.NameIdentifier, userResponse.Id.ToString())
};




		ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

		await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

		TempData["Success"] = "Giriş işlemi başarılı";

		return RedirectToAction("List", "Blog");
	}

	public async Task<IActionResult> Logout() {
		await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		TempData["Success"] = "Çıkış işlemi başarılı.";
		return RedirectToAction("Login", "Auth");
	}

}
