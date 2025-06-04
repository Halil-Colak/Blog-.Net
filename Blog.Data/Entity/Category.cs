using Blog.Data.Entity.Abstract;
using Blog.Data.Entity.Junction;

namespace Blog.Data.Entity;
public class Category : BaseEntity {
	public string Name { get; set; }
	public string Slug { get; set; }  // SEO uyumlu URL
	public List<BlogCategory> BlogCategorys { get; set; }

}
