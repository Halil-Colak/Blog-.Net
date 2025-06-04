using Blog.Data.Entity.Abstract;
using Blog.Data.Entity.Junction;

namespace Blog.Data.Entity;
public class Blog : BaseEntity {
	public string Title { get; set; }
	public string Slug { get; set; }  // SEO uyumlu URL
	public string Description { get; set; }
	public string Content { get; set; }
	public string ImageUrl { get; set; }  // Kapak fotoğrafı
	public bool IsPopular { get; set; } = false;
	public int UserId { get; set; }
	public User User { get; set; }
	public List<BlogCategory> BlogCategorys { get; set; }
	public List<BlogLike> BlogLikes { get; set; }  // SEO uyumlu URL
	public List<Comment> Comments { get; set; }  // SEO uyumlu URL

}

