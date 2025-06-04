using Blog.Data.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BaseContext>(options => {
	ConfigurationManager config = builder.Configuration;
	string connectionString = config.GetConnectionString("Connection");
	options.UseSqlServer(connectionString);
});
// Add services to the container.
builder.Services.AddControllersWithViews();


// Add Authentication
builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(opts => {
	opts.Cookie.Name = ".base.auth";
	opts.AccessDeniedPath = "/Auth/Login"; // Unauthorized access redirect path
	opts.LoginPath = "/Auth/Login"; // Login path
	opts.SlidingExpiration = true;
	opts.ExpireTimeSpan = TimeSpan.FromDays(3); // Cookie expiration time
});

// IHttpContextAccessor servisini ekleyin
builder.Services.AddHttpContextAccessor();



WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentication middleware burada çaðrýlmalý
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Category}/{action=List}/{id?}");

app.Run();
