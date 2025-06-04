using Blog.Proxy;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 HttpClient servisini ekle (proxy için gerekli)
builder.Services.AddHttpClient();

// 🌐 React uygulamasından gelen isteklere izin ver (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5175") // React dev URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
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

// Configure the HTTP request pipeline.
// Swagger sadece geliştirme ortamında çalışmalı
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔓 CORS middleware'ini ilk sırada ekleyin
app.UseCors("AllowFrontend");  // CORS, Authorization ve diğer middleware'lerden önce gelir.
app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
