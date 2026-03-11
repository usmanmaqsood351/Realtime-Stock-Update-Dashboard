using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Application.Models;
using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Application.Services;

public class StockService
{
    private readonly IStockRepository _repository;

    public StockService(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<StockDto?> UpdateAsync(int id, StockDto input, CancellationToken cancellationToken)
    {
        var item = new StockItem
        {
            Id = id,
            Symbol = input.Symbol.Trim().ToUpperInvariant(),
            Company = input.Company.Trim(),
            Price = input.Price,
            Shares = input.Shares,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var updated = await _repository.UpdateAsync(item, cancellationToken);
        return updated is null ? null : MapToDto(updated);
    }
    private static StockDto MapToDto(StockItem item)
    {
        return new StockDto
        {
            Id = item.Id,
            Symbol = item.Symbol,
            Company = item.Company,
            Price = item.Price,
            Shares = item.Shares,
            UpdatedAtUtc = item.UpdatedAtUtc
        };
    }
}
