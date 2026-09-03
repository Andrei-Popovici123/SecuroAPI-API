using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SecuroAPI_API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        => (_logger, _env) = (logger, env);

    public async ValueTask<bool> TryHandleAsync(
        HttpContext ctx, Exception ex, CancellationToken ct)
    {
        var traceId = ctx.TraceIdentifier;

        _logger.LogError(ex,
            "Unhandled exception {TraceId} on {Method} {Path}",
            traceId, ctx.Request.Method, ctx.Request.Path);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = _env.IsDevelopment() ? ex.ToString() : null,
            Instance = ctx.Request.Path
        };
        problem.Extensions["traceId"] = traceId;

        ctx.Response.StatusCode = problem.Status.Value;
        await ctx.Response.WriteAsJsonAsync(problem, ct);

        return true;
    }
}