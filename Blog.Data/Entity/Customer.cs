using Blog.Data.Entity.Abstract;
using Blog.Data.Entity.Junction;

namespace Blog.Data.Entity;
public class Customer : BaseEntity
{
    public string Name { get; set; }
    public string SurnName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Slug { get; set; }  // SEO uyumlu URL
    public string? ProfilPhoto { get; set; }
    public string? EmailVerifyToken { get; set; }
    public string? ResetPasswordToken { get; set; }
    public List<BlogLike> BlogLikes { get; set; }  // SEO uyumlu URL
    public List<Comment> Comments { get; set; }  // SEO uyumlu URL

}

