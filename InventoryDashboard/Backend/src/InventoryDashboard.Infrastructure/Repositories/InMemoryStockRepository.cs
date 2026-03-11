using System.Collections.Concurrent;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Infrastructure.Repositories;

public class InMemoryStockRepository : IStockRepository
{
    private readonly ConcurrentDictionary<int, StockItem> _store = new();
    private int _nextId;

    public InMemoryStockRepository(IEnumerable<StockItem> seed)
    {
        foreach (var item in seed)
        {
            var id = item.Id == 0 ? ++_nextId : item.Id;
            var copy = Clone(item);
            copy.Id = id;
            _store[id] = copy;
            _nextId = Math.Max(_nextId, id);
        }
    }

    public Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<StockItem> items = _store.Values
            .OrderBy(p => p.Id)
            .Select(Clone)
            .ToList();
        return Task.FromResult(items);
    }

    public Task<StockItem?> UpdateAsync(StockItem item, CancellationToken cancellationToken)
    {
        if (!_store.ContainsKey(item.Id))
        {
            return Task.FromResult<StockItem?>(null);
        }

        var copy = Clone(item);
        copy.UpdatedAtUtc = DateTime.UtcNow;
        _store[copy.Id] = copy;
        return Task.FromResult<StockItem?>(Clone(copy));
    }

    private static StockItem Clone(StockItem item)
    {
        return new StockItem
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
