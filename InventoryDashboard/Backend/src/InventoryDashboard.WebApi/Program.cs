using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Application.Services;
using InventoryDashboard.Infrastructure.Repositories;
using InventoryDashboard.Infrastructure.Seed;
using InventoryDashboard.WebApi.Background;
using InventoryDashboard.WebApi.Hubs;
using InventoryDashboard.WebApi.Middleware;
using InventoryDashboard.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();

builder.Services.AddSingleton<IStockRepository>(_ =>
    new InMemoryStockRepository(DataSeeder.SeedStocks()));

builder.Services.AddScoped<StockService>();
builder.Services.AddSingleton<StockTickerService>();
builder.Services.AddHostedService<StockTickerHostedService>();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseMiddleware<RandomFailureMiddleware>();

app.MapControllers();
app.MapHub<InventoryHub>("/hub/inventory");

app.Run();
