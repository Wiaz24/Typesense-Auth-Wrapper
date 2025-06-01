using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TypesenseAuthWrapper.Bootstrapper.TypesenseMiddleware;

public class TypesenseMiddleware : IMiddleware
{
    private readonly TypesenseOptions _options;
    private readonly ILogger<TypesenseMiddleware> _logger;
    private readonly HttpClient _httpClient;

    public TypesenseMiddleware(IOptions<TypesenseOptions> options,
        ILogger<TypesenseMiddleware> logger,
        IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("Typesense");
        _options = options.Value;
        _logger = logger;
    }
    
    private Uri BuildTypesenseUrl(PathString path, string queryString)
    {
        var baseUrl = _options.Url.TrimEnd('/');
        var relativePath = path.ToString().TrimStart('/');
        if (relativePath.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Substring(4); // Remove "api/" prefix
        }
        var fullUrl = $"{baseUrl}/{relativePath}{queryString}";
        return new Uri(fullUrl);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            bool hasAuthorizationHeader = context.Request.Headers.ContainsKey("Authorization");
            if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Request path does not match Typesense API");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Not Found - this endpoint is not available");
                return;
            }
            if (!hasAuthorizationHeader)
            {
                _logger.LogWarning("Request received without Authorization header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authentication required - please provide a valid JWT token");
                return;
            }
            
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("Unauthorized request received");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid or expired token - please provide a valid JWT token");
                return;
            }
            var typesenseRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(context.Request.Method),
                RequestUri = BuildTypesenseUrl(context.Request.Path, context.Request.QueryString.ToString())
            };
            
            foreach (var header in context.Request.Headers)
            {
                if (header.Key.ToLower() is not ("host" or "authorization"))
                {
                    typesenseRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }
            
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                var buffer = new byte[context.Request.ContentLength.Value];
                await context.Request.Body.ReadExactlyAsync(buffer, 0, buffer.Length);
                typesenseRequest.Content = new ByteArrayContent(buffer);
                
                if (context.Request.ContentType != null)
                {
                    typesenseRequest.Content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType);
                }
                
                context.Request.Body.Position = 0;
            }
            
            var response = await _httpClient.SendAsync(typesenseRequest);
            
            foreach (var header in response.Headers)
            {
                if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }
            }
            
            foreach (var header in response.Content.Headers)
            {
                if (!header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            context.Response.StatusCode = (int)response.StatusCode;
            
            var responseBody = await response.Content.ReadAsByteArrayAsync();
            await context.Response.Body.WriteAsync(responseBody, 0, responseBody.Length);

            _logger.LogInformation($"Request forwarded to Typesense. Status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while forwarding request to Typesense");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An error occurred while processing your request");
        }
    }
}