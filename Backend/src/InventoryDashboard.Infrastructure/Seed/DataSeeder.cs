using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Infrastructure.Seed;

public static class DataSeeder
{
    public static IReadOnlyList<StockItem> SeedStocks()
    {
        var now = DateTime.UtcNow;
        return new List<StockItem>
        {
            new() { Id = 1, Symbol = "MSFT", Company = "Microsoft", Price = 412.35m, Shares = 120, UpdatedAtUtc = now },
            new() { Id = 2, Symbol = "AAPL", Company = "Apple", Price = 188.22m, Shares = 300, UpdatedAtUtc = now },
            new() { Id = 3, Symbol = "AMZN", Company = "Amazon", Price = 174.03m, Shares = 85, UpdatedAtUtc = now },
            new() { Id = 4, Symbol = "NVDA", Company = "NVIDIA", Price = 870.12m, Shares = 42, UpdatedAtUtc = now },
            new() { Id = 5, Symbol = "META", Company = "Meta", Price = 468.51m, Shares = 65, UpdatedAtUtc = now },
            new() { Id = 6, Symbol = "GOOGL", Company = "Alphabet", Price = 149.76m, Shares = 95, UpdatedAtUtc = now },
            new() { Id = 7, Symbol = "TSLA", Company = "Tesla", Price = 184.44m, Shares = 140, UpdatedAtUtc = now },
            new() { Id = 8, Symbol = "ORCL", Company = "Oracle", Price = 123.77m, Shares = 210, UpdatedAtUtc = now },
            new() { Id = 9, Symbol = "ADBE", Company = "Adobe", Price = 515.62m, Shares = 55, UpdatedAtUtc = now },
            new() { Id = 10, Symbol = "INTC", Company = "Intel", Price = 45.18m, Shares = 400, UpdatedAtUtc = now }
        };
    }
}
