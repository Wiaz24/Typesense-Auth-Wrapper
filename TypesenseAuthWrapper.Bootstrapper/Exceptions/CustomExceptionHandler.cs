using Microsoft.AspNetCore.Diagnostics;

namespace TypesenseAuthWrapper.Bootstrapper.Exceptions;

public class CustomExceptionHandler : IMiddleware
{
    private readonly ILogger<CustomExceptionHandler> _logger;
    private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger,
        IEnumerable<IExceptionHandler> exceptionHandlers)
    {
        _logger = logger;
        _exceptionHandlers = exceptionHandlers;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            foreach (var handler in _exceptionHandlers)
            {
                if (await handler.TryHandleAsync(context, ex, context.RequestAborted))
                {
                    return;
                }
            }

            throw;
        }
    }
}