namespace Blog.Proxy;
using Microsoft.Extensions.Primitives;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string API_KEY_HEADER = "X-API-KEY";
    private readonly string _apiKeyValue;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _apiKeyValue = config.GetValue<string>("ApiKey");
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/favicon.ico") ||
            context.Request.Path.StartsWithSegments("/upload"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out StringValues extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key eksik");
            return;
        }

        if (string.IsNullOrEmpty(_apiKeyValue) || !_apiKeyValue.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Geçersiz API Key");
            return;
        }

        await _next(context);
    }
}
