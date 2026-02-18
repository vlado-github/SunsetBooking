using FluentValidation;
using SunsetBooking.API.Middlewares.CustomResponses;
using SunsetBooking.Domain.Shared.Exceptions;

namespace SunsetBooking.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, 
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(context, exception);
            _logger.LogWarning(exception, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            await HandleInvalidOperationExceptionAsync(context, exception);
            _logger.LogWarning(exception, exception.Message);
        }
        catch (RecordNotFoundException exception)
        {
            await HandleNotFoundExceptionAsync(context, exception);
            _logger.LogWarning(exception, exception.Message);
        }
    }

    private async Task HandleNotFoundExceptionAsync(HttpContext context, RecordNotFoundException exception)
    {
        var errorDetails = new NotFoundErrorDetails(exception.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDetails.Status;
        await context.Response.WriteAsync(errorDetails.ToString());
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var validationDetails = new CustomValidationErrorDetails(exception.Message, exception.Errors);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = validationDetails.Status;
        await context.Response.WriteAsync(validationDetails.ToString());
    }
    
    private async Task HandleInvalidOperationExceptionAsync(HttpContext context, InvalidOperationException exception)
    {
        var errorDetails = new InvalidOperationErrorDetails(exception.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorDetails.Status;
        await context.Response.WriteAsync(errorDetails.ToString());
    }
}