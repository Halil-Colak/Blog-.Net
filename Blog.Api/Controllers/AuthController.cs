using Blog.Data.Context;
using Blog.Data.Entity;
using Blog.Helper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Web;

namespace Blog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly BaseContext _context;
    private readonly IConfiguration _config;

    public AuthController(BaseContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }


    [HttpPost("Register")]

    public async Task<IActionResult> Register([FromForm] string name = "", [FromForm] string surName = "", [FromForm] string email = "", [FromForm] string password = "")
    {
        if (name == "" || surName == "" || email == "" || password == "")
            return Ok(new ServiceResponse { Status = false, Message = "Boş olamaz." });

        bool customerSlug = _context.Customers.Any(x => x.Slug == SeoUrl.Url(name + " " + surName));
        if (customerSlug)
            return Ok(new ServiceResponse { Status = false, Message = "Bu ada sahip kullanıcı bulunmakta" });

        bool customerEmail = _context.Customers.Any(x => x.Email == email);
        if (customerEmail)
            return Ok(new ServiceResponse { Status = false, Message = "Bu emaile sahip kullanıcı bulunmakta" });

        string token = Guid.NewGuid().ToString();
        string slug = SeoUrl.Url(name + " " + surName);

        Customer customer = new()
        {
            Name = name,
            SurnName = surName,
            Email = email,
            Password = BCryptHash.BCryptHashPassword(password),
            Slug = slug,
            IsActive = false,
            EmailVerifyToken = token
        };

        _context.Customers.Add(customer);
        _context.SaveChanges();

        // Doğrulama linki oluştur
        //string confirmLink = $"http://localhost:5175/eposta-dogrula?email={email}&token={HttpUtility.UrlEncode(token)}";
        string confirmLink = $"https://hb.hctheme.com/eposta-dogrula?email={email}&token={HttpUtility.UrlEncode(token)}";

        // SMTP ayarları
        string? smtpServer = _config["EmailSettings:SmtpServer"];
        int smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
        string? senderEmail = _config["EmailSettings:SenderEmail"];
        string? senderPassword = _config["EmailSettings:SenderPassword"];

        MimeMessage emailMessage = new();
        emailMessage.From.Add(MailboxAddress.Parse(senderEmail));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = "Email Doğrulama";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"<p>Merhaba {name},</p><p>Hesabını doğrulamak için <a href='{confirmLink}'>buraya tıkla</a>.</p>"
        };

        using SmtpClient smtp = new();
        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(emailMessage);
        await smtp.DisconnectAsync(true);

        return Ok(new ServiceResponse { Message = "Kayıt başarılı. Lütfen e-postanı doğrula." });
    }

    [HttpGet("ConfirmEmail")]
    public IActionResult ConfirmEmail(string email, string token)
    {
        Customer? customer = _context.Customers.FirstOrDefault(x => x.Email == email && x.EmailVerifyToken == token);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Doğrulama başarısız." });

        customer.IsActive = true;
        customer.EmailVerifyToken = null;
        _context.SaveChanges();

        return Ok(new ServiceResponse { Message = "Email başarıyla doğrulandı." });

    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] string email = "", [FromForm] string password = "")
    {
        if (email == "" || password == "")
            return Ok(new ServiceResponse { Status = false, Message = "Boş olamaz." });

        Customer? customer = _context.Customers.FirstOrDefault(x => x.Email == email && x.IsActive);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı veya doğrulanmamış." });

        bool customePassControll = BCryptHash.VerifyPassword(password, customer.Password);
        if (!customePassControll)
            return Ok(new ServiceResponse { Status = false, Message = "Kullanıcı bulunamadı" });

        var customerTarget = new
        {
            Name = customer.Name,
            SurnName = customer.SurnName,
            Email = email,
            Slug = customer.Slug,
            ProfilPhoto = customer.ProfilPhoto
        };

        return Ok(new ServiceResponse { Message = "Giriş işlemi başarılı", Data = customerTarget });
    }


    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromForm] string email)
    {
        Customer? customer = _context.Customers.FirstOrDefault(x => x.Email == email);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Bu e-posta adresine ait kullanıcı bulunamadı." });


        string resetToken = Guid.NewGuid().ToString();

        customer.ResetPasswordToken = resetToken;
        _context.SaveChanges();

        // Şifre sıfırlama linki oluşturuyoruz
        //string resetLink = $"http://localhost:5175/sifre-sifirla?email={email}&token={HttpUtility.UrlEncode(resetToken)}";
        string resetLink = $"https://hb.hctheme.com/sifre-sifirla?email={email}&token={HttpUtility.UrlEncode(resetToken)}";


        // SMTP ayarları
        string smtpServer = _config["EmailSettings:SmtpServer"];
        int smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
        string senderEmail = _config["EmailSettings:SenderEmail"];
        string senderPassword = _config["EmailSettings:SenderPassword"];

        MimeMessage emailMessage = new();
        emailMessage.From.Add(MailboxAddress.Parse(senderEmail));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = "Şifre Sıfırlama Linki";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"<p>Merhaba,</p><p>Şifrenizi sıfırlamak için <a href='{resetLink}'>buraya tıklayın</a>.</p>"
        };

        // E-posta gönderme
        using SmtpClient smtp = new();
        await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(senderEmail, senderPassword);
        await smtp.SendAsync(emailMessage);
        await smtp.DisconnectAsync(true);

        return Ok(new ServiceResponse { Message = "Şifre sıfırlama linki e-posta adresinize gönderildi." });
    }

    [HttpPost("ResetPassword")]
    public IActionResult ResetPassword([FromForm] string email, [FromForm] string token, [FromForm] string newPassword)
    {
        Customer? customer = _context.Customers.FirstOrDefault(x => x.Email == email && x.ResetPasswordToken == token);
        if (customer == null)
            return Ok(new ServiceResponse { Status = false, Message = "Geçersiz token veya e-posta." });

        customer.Password = BCryptHash.BCryptHashPassword(newPassword);
        customer.ResetPasswordToken = null;
        _context.SaveChanges();

        return Ok(new ServiceResponse { Message = "Şifreniz başarıyla sıfırlandı." });
    }

}
