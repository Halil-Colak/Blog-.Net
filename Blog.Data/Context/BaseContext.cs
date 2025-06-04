using Blog.Data.Entity;
using Blog.Data.Entity.Junction;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Context;
public class BaseContext : DbContext
{
    public BaseContext(DbContextOptions<BaseContext> options) : base(options)
    {
    }
    public BaseContext() { }
    public DbSet<User> Users { get; set; }
    public DbSet<Entity.Blog> Blogs { get; set; }
    public DbSet<BlogLike> BlogLikes { get; set; }
    public DbSet<BlogCategory> BlogCategorys { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Report> Reports { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
    //	.UseSqlServer("Server=DESKTOP-KFTKIUU\\HALIL;Database=Blog;Trusted_Connection=True;TrustServerCertificate=True");

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
    //.UseSqlServer("Server=DESKTOP-BCBS3QD\\SQLEXPRESS;Database=Blog;Trusted_Connection=True;TrustServerCertificate=True");


    #region Canlı ÇALISAN URL
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=89.252.183.170\\MSSQLSERVER2019;Database=hcthemec_Blog;user=hcthemec_HalilBlog;password=_xga8A829;TrustServerCertificate=true;");
    }
    #endregion

}
