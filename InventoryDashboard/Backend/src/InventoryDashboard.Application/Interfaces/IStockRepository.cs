using InventoryDashboard.Application.Models;
using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Application.Interfaces;

public interface IStockRepository
{
    Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<StockItem?> UpdateAsync(StockItem item, CancellationToken cancellationToken);
}
