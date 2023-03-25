namespace PsychToGoMVC.Middleware;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next( httpContext );
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _logger.LogError( "{ExceptionType} {ExceptionMessage}",
                    ex.InnerException.GetType().ToString(),
                    ex.InnerException.Message );
            }
            else
            {
                _logger.LogError( "{ExceptionType} {ExceptionMessage}",
                    ex.GetType().ToString(),
                    ex.Message );
            }
            await httpContext.Response.WriteAsync( "Something went wrong. " );
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}