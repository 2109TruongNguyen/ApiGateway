namespace OcelotGateway.Middlewares;

public class TokenCheckerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value!.ToLower();

        var isPublic = path.Contains("/auth/login") || 
                      path.Contains("/auth/register") || 
                      path.Contains("/public") || 
                      path.Contains("google") || 
                      path.Contains("/swagger") ||
                      path.Contains("/otp/generate");

        var hasAuth = context.Request.Headers.TryGetValue("Authorization", out var token);

        if (!isPublic && !hasAuth)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing token");
            return;
        }

        await next(context);
    }
}
