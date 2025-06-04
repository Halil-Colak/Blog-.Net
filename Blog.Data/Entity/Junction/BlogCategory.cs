using Blog.Data.Entity.Abstract;

namespace Blog.Data.Entity.Junction;
public class BlogCategory : BaseEntity {
	public int CategoryId { get; set; }
	public Category Category { get; set; }

	public int BlogId { get; set; }
	public Blog Blog { get; set; }
}
