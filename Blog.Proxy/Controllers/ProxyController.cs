using Microsoft.AspNetCore.Mvc;

namespace Blog.Proxy.Controllers
{
    [ApiController]
    [Route("api/{**path}")]
    public class ProxyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProxyController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
        public async Task<IActionResult> ProxyRequest(string path)
        {
            string? apiKey = _configuration["ApiKey"];
            HttpClient client = _httpClientFactory.CreateClient();

            string externalApiUrl = "https://localhost:7276/";

            // Query string'i mevcut isteğin URL'sinden al
            string queryString = Request.QueryString.ToString();

            // Yeni istek oluştur
            UriBuilder uriBuilder = new($"{externalApiUrl}{path}")
            {
                Query = queryString
            };

            Uri requestUri = uriBuilder.Uri;

            HttpRequestMessage requestMessage = new()
            {
                Method = new HttpMethod(Request.Method),
                RequestUri = requestUri
            };

            // API key'i header'dan gönder
            if (!string.IsNullOrEmpty(apiKey))
            {
                requestMessage.Headers.Add("x-api-key", apiKey);
            }
            else
            {
                return BadRequest("API key eksik.");
            }

            if (Request.ContentLength > 0)
            {
                using StreamReader stream = new(Request.Body);
                string body = await stream.ReadToEndAsync();
                requestMessage.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            string content = await response.Content.ReadAsStringAsync();
            return Content(content, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }
    }
}
