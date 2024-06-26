namespace intex2.wwwroot.Middleware;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "img-src 'self' https:; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline';" +
            "object-src 'none';";

        await _next(context);
    }
}