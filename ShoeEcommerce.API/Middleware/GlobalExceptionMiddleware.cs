using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ShoeEcommerce.Application.Common.Exceptions;

namespace ShoeEcommerce.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Structured log with path and trace id for easier troubleshooting
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception for request {Method} {Path} TraceId: {TraceId}",
                context.Request?.Method,
                context.Request?.Path.Value,
                traceId);

            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        HttpStatusCode statusCode;
        object response;

        switch (exception)
        {
            case ValidationException ve:
                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    message = ve.Message,
                    errors = ve.Errors,
                    traceId
                };
                break;

            case UnauthorizedException ue:
                statusCode = HttpStatusCode.Unauthorized;
                response = new { message = ue.Message, traceId };
                break;

            case InvalidRefreshTokenException ir:
                statusCode = HttpStatusCode.Unauthorized;
                response = new { message = ir.Message, traceId };
                break;

            case ForbiddenException _:
            case EmailNotConfirmedException _:
                statusCode = HttpStatusCode.Forbidden;
                response = new { message = exception.Message, traceId };
                break;

            case AccountLockedException al:
                // 423 Locked doesn't exist as HttpStatusCode enum member pre-.NET 7, cast explicitly
                statusCode = (HttpStatusCode)423;
                response = new
                {
                    message = al.Message,
                    lockoutEnd = al.LockoutEnd,
                    traceId
                };
                break;

            case AccountBlockedException ab:
                statusCode = (HttpStatusCode)423;
                response = new
                {
                    message = ab.Message,
                    reason = ab.Reason,
                    expiresAt = ab.ExpiresAt,
                    traceId
                };
                break;

            case NotFoundException nf:
                statusCode = HttpStatusCode.NotFound;
                response = new { message = nf.Message, traceId };
                break;

            case ConflictException ce:
                statusCode = HttpStatusCode.Conflict;
                response = new { message = ce.Message, traceId };
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                if (_env.IsDevelopment())
                {
                    // Helpful for dev: include exception message + stack trace
                    response = new
                    {
                        message = exception.Message,
                        detail = exception.StackTrace,
                        traceId
                    };
                }
                else
                {
                    // Production-friendly: generic message only
                    response = new { message = "An unexpected error occurred", traceId };
                }
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }
}
