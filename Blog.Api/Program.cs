using Blog.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// CORS politikası ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});


// ConfigurationManager ile appsettings.json erişimi
ConfigurationManager config = builder.Configuration;

// DbContext yapılandırması
builder.Services.AddDbContext<BaseContext>(options =>
{
    string connectionString = config.GetConnectionString("Connection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger yapılandırması (API Key ile güvenlik ekleme)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // API Key zorunlu olsun
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, // API Key başlıkta gönderilecek
        Name = "X-API-KEY", // Kullanıcının girmesi gereken API Key başlığı
        Type = SecuritySchemeType.ApiKey,
        Description = "API'yi kullanmak için API Key giriniz."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

WebApplication app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseStaticFiles();

app.Run();
