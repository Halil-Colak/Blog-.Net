using Blog.Data.Entity.Abstract;

namespace Blog.Data.Entity;
public class User : BaseEntity {
	public string Name { get; set; }
	public string Surname { get; set; }
	public string Email { get; set; }
	public string PaswortHas { get; set; }
	public string ProfileImage { get; set; }
	public List<Blog> Blogs { get; set; }

}