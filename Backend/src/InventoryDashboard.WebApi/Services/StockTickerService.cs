using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Application.Models;

namespace InventoryDashboard.WebApi.Services;

public class StockTickerService
{
    private readonly IStockRepository _repository;
    private readonly Random _random = new();

    public StockTickerService(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<StockDto?> TickAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        if (items.Count == 0)
        {
            return null;
        }

        var pick = items[_random.Next(items.Count)];
        var deltaPercent = (decimal)(_random.NextDouble() * 2.0 - 1.0) * 0.02m; // -2% to +2%
        var newPrice = Math.Max(0.01m, pick.Price + pick.Price * deltaPercent);
        var newShares = Math.Max(0, pick.Shares + _random.Next(-10, 11));

        var updated = await _repository.UpdateAsync(new Domain.Entities.StockItem
        {
            Id = pick.Id,
            Symbol = pick.Symbol,
            Company = pick.Company,
            Price = Math.Round(newPrice, 2),
            Shares = newShares,
            UpdatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        if (updated is null)
        {
            return null;
        }

        return new StockDto
        {
            Id = updated.Id,
            Symbol = updated.Symbol,
            Company = updated.Company,
            Price = updated.Price,
            Shares = updated.Shares,
            UpdatedAtUtc = updated.UpdatedAtUtc
        };
    }
}
