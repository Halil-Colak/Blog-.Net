using Blog.Data.Entity.Abstract;

namespace Blog.Data.Entity;
public class Comment : BaseEntity {
	public string Content { get; set; }

	public int BlogId { get; set; }
	public Blog Blog { get; set; }

	// Yorum yapan müşteri (nullable)
	public int? CustomerId { get; set; }
	public Customer Customer { get; set; }

	// Yorum yapan admin / yazar (nullable)
	public int? UserId { get; set; }
	public User User { get; set; }

	public int? ParentCommentId { get; set; }
	public Comment ParentComment { get; set; }

	public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}

