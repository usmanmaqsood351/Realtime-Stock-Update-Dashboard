# Solution Overview

This repository contains two separate solutions:

- `Backend/` - ASP.NET Core 8 Web API with Clean Architecture style layering.
- `frontend/` - React single-page app (Create React App) with an optimized, sortable grid and SignalR live updates.

A `docker-compose.yml` at the repo root runs both together.

## Backend Architecture (Clean-ish and Simple)

Layers:

- **Domain**: Pure entity models.
- **Application**: Interfaces + DTOs + simple service logic.
- **Infrastructure**: In-memory repository + seed data.
- **WebApi**: Controllers, SignalR hub, middleware, configuration, background ticker.

### Class-by-class walkthrough

**Domain**

- `Entities/StockItem.cs`
  - Holds the stock model: `Id`, `Symbol`, `Company`, `Price`, `Shares`, `UpdatedAtUtc`.
  - No dependencies on other layers.

**Application**

- `Interfaces/IStockRepository.cs`
  - Contract for data access (list, update).
  - Keeps Application and WebApi independent from storage details.

- `Models/StockDto.cs`
  - Data transfer model used by controllers and UI.

- `Services/StockService.cs`
  - Orchestrates stock operations.
  - Maps between DTOs and domain entities.
  - Normalizes `Symbol` to uppercase.

**Infrastructure**

- `Repositories/InMemoryStockRepository.cs`
  - Simple, thread-safe `ConcurrentDictionary` store.
  - Implements `IStockRepository`.
  - `Clone` method prevents accidental shared references.

- `Seed/DataSeeder.cs`
  - Provides sample stock records on application start.

**WebApi**

- `Controllers/StocksController.cs`
  - REST endpoint for stock list: `GET /api/stocks`.

- `Hubs/InventoryHub.cs`
  - SignalR hub for pushing live updates to the UI.

- `Services/StockTickerService.cs`
  - Picks a random stock and nudges price (+/-2%) and shares.

- `Background/StockTickerHostedService.cs`
  - Runs every `StockTicker:IntervalSeconds` (default 5s).
  - Broadcasts `stockUpdated` to SignalR clients.

- `Middleware/RandomFailureMiddleware.cs`
  - Injects random failures (default 10%).
  - Skips `OPTIONS` and SignalR hub routes.

- `Program.cs`
  - Wires DI, SignalR, CORS, middleware pipeline, and hosted service.

### API Endpoints

- `GET /api/stocks`
- SignalR hub: `/hub/inventory`

## Frontend Architecture (Create React App)

- `src/App.js`
  - Fetches stocks from the API (Axios).
  - Connects to SignalR and applies live updates.
  - Sorts grid data by clicking column headers.
  - Includes a Refresh button (no simulate button).

- `src/components/DataGrid.jsx`
  - Uses `react-window` virtualization for an optimized grid.
  - Memoized rows to reduce re-render work.

- `src/services/api.js`
  - API client wrapper with Axios.

- `src/services/signalr.js`
  - SignalR connection builder with auto-reconnect.

## Architectural Trade-offs

- **In-memory data**: No database to keep setup simple and fast. Data resets on restart.
- **Background ticker**: Server pushes updates every few seconds; this keeps the UI live without manual triggers.
- **Random failure middleware**: Implemented at the middleware level to simulate flaky APIs.
- **DTOs over Entities**: Keeps the contract stable even if the domain model evolves.
- **React-window grid**: Lightweight optimization without building a full grid engine.

## Running Locally

Backend:

```bash
cd Backend
dotnet restore
dotnet run --project src/InventoryDashboard.WebApi
```

Frontend:

```bash
cd frontend
npm install
npm start
```

The frontend expects the API at `http://localhost:5000` by default (set in `frontend/.env`).

## Docker

From repository root:

```bash
docker-compose up --build
```

- Frontend: `http://localhost:5173`
- Backend API: `http://localhost:5000`

## GitHub Link


