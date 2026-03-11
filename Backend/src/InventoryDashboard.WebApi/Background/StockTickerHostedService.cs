using InventoryDashboard.WebApi.Hubs;
using InventoryDashboard.WebApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace InventoryDashboard.WebApi.Background;

public class StockTickerHostedService : BackgroundService
{
    private readonly StockTickerService _ticker;
    private readonly IHubContext<InventoryHub> _hub;
    private readonly ILogger<StockTickerHostedService> _logger;
    private readonly TimeSpan _interval;

    public StockTickerHostedService(
        StockTickerService ticker,
        IHubContext<InventoryHub> hub,
        IConfiguration configuration,
        ILogger<StockTickerHostedService> logger)
    {
        _ticker = ticker;
        _hub = hub;
        _logger = logger;
        var seconds = configuration.GetValue<int?>("StockTicker:IntervalSeconds") ?? 5;
        _interval = TimeSpan.FromSeconds(Math.Max(1, seconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var updated = await _ticker.TickAsync(stoppingToken);
                if (updated is not null)
                {
                    await _hub.Clients.All.SendAsync("stockUpdated", updated, stoppingToken);
                    _logger.LogInformation("Ticked {Symbol} at {Time}.", updated.Symbol, updated.UpdatedAtUtc);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish stock tick.");
            }
        }
    }
}
