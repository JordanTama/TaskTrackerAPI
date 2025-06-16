using System.Text.Json;

namespace TaskTrackerAPI.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            await context.Response.WriteAsync(result);
        }
    }
}