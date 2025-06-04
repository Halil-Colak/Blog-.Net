using System.ComponentModel.DataAnnotations;

namespace Blog.Data.Entity.Abstract;
public class BaseEntity {
	[Key]
	public int Id { get; set; }
	public DateTime CreatedDateTime { get; set; } = DateTime.Now;
	public bool IsActive { get; set; } = true;
}

