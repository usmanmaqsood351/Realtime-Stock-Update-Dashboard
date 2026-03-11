using InventoryDashboard.Application.Models;
using InventoryDashboard.Application.Services;
using InventoryDashboard.WebApi.Hubs;
using InventoryDashboard.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace InventoryDashboard.WebApi.Controllers;

[ApiController]
[Route("api/stocks")]
public class StocksController : ControllerBase
{
    private readonly StockService _service;
    private readonly StockTickerService _ticker;
    private readonly IHubContext<InventoryHub> _hub;

    public StocksController(StockService service, StockTickerService ticker, IHubContext<InventoryHub> hub)
    {
        _service = service;
        _ticker = ticker;
        _hub = hub;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken);
        return Ok(items);
    }
}
