using Blog.Data.Entity.Abstract;

namespace Blog.Data.Entity.Junction;
public class BlogLike : BaseEntity {
	public Customer Customer { get; set; }
	public int CustomerId { get; set; }
	public Blog Blog { get; set; }
	public int BlogId { get; set; }
}
