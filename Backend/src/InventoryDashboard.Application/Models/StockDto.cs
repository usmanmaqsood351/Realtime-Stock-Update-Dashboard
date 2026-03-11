namespace InventoryDashboard.Application.Models;

public class StockDto
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Shares { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
