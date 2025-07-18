using System.Text;

namespace Web.Middlewares;

public class SwaggerAuthMiddleware(RequestDelegate next, IConfiguration config)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path is not null && !path.StartsWith("/swagger") && !path.StartsWith("/api/specification.json"))
        {
            await next(context);
            return;
        }

        var username = config["SwaggerAuth:Username"];
        var password = config["SwaggerAuth:Password"];

        string authHeader = context.Request.Headers["Authorization"].ToString();

        if (authHeader.StartsWith("Basic "))
        {
            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var parts = decodedCredentials.Split(':');

            if (parts.Length == 2 && parts[0] == username && parts[1] == password)
            {
                await next(context);
                return;
            }
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
}
