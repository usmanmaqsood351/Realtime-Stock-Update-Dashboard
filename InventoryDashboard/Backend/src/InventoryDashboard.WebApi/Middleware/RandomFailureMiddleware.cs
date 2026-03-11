using System.Text.Json;

namespace InventoryDashboard.WebApi.Middleware;

public class RandomFailureMiddleware
{
    private static readonly Random _random = new();
    private readonly RequestDelegate _next;
    private readonly ILogger<RandomFailureMiddleware> _logger;
    private readonly int _failureRatePercent;

    public RandomFailureMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RandomFailureMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _failureRatePercent = configuration.GetValue<int?>("RandomFailureRatePercent") ?? 10;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkip(context))
        {
            await _next(context);
            return;
        }

        if (ShouldFail())
        {
            _logger.LogWarning("Random failure injected for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new
            {
                message = "Random failure injected for testing.",
                status = 500
            });

            await context.Response.WriteAsync(payload);
            return;
        }

        await _next(context);
    }

    private bool ShouldFail()
    {
        lock (_random)
        {
            return _random.Next(0, 100) < _failureRatePercent;
        }
    }

    private static bool ShouldSkip(HttpContext context)
    {
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            return true;
        }

        return context.Request.Path.StartsWithSegments("/hub");
    }
}
