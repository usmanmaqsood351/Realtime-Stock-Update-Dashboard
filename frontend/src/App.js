import React, { useCallback, useEffect, useMemo, useState } from "react";
import DataGrid from "./components/DataGrid";
import { fetchStocks } from "./services/api";
import { createInventoryConnection } from "./services/signalr";
import "./App.css";

function App() {
  const [stocks, setStocks] = useState([]);
  const [status, setStatus] = useState("disconnected");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [sortKey, setSortKey] = useState("id");
  const [sortDirection, setSortDirection] = useState("asc");

  const loadStocks = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const data = await fetchStocks();
      setStocks(data);
    } catch (err) {
      setError("Failed to load stocks. Random failure may have occurred.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadStocks();
  }, [loadStocks]);

  useEffect(() => {
    const connection = createInventoryConnection();
    setStatus("connecting");

    connection.on("stockCreated", (stock) => {
      setStocks((prev) => {
        if (prev.some((item) => item.id === stock.id)) {
          return prev;
        }
        return [...prev, stock].sort((a, b) => a.id - b.id);
      });
    });

    connection.on("stockUpdated", (stock) => {
      setStocks((prev) => prev.map((item) => (item.id === stock.id ? stock : item)));
    });

    connection.on("stockDeleted", (id) => {
      setStocks((prev) => prev.filter((item) => item.id !== id));
    });

    connection
      .start()
      .then(() => setStatus("connected"))
      .catch(() => setStatus("disconnected"));

    connection.onreconnected(() => setStatus("connected"));
    connection.onreconnecting(() => setStatus("reconnecting"));
    connection.onclose(() => setStatus("disconnected"));

    return () => {
      connection.stop();
    };
  }, []);

  const columns = useMemo(
    () => [
      { key: "id", label: "ID", width: "70px", sortValue: (row) => row.id },
      { key: "symbol", label: "Symbol", width: "110px", sortValue: (row) => row.symbol },
      { key: "company", label: "Company", width: "1.6fr", sortValue: (row) => row.company },
      {
        key: "price",
        label: "Price",
        width: "120px",
        format: (value) => `$${Number(value).toFixed(2)}`,
        sortValue: (row) => row.price
      },
      { key: "shares", label: "Shares", width: "110px", sortValue: (row) => row.shares },
      {
        key: "updatedAtUtc",
        label: "Last Updated",
        width: "1.4fr",
        format: (value) => new Date(value).toLocaleString(),
        sortValue: (row) => new Date(row.updatedAtUtc).getTime()
      }
    ],
    []
  );

  const handleSort = useCallback(
    (key) => {
      setSortKey((current) => {
        if (current !== key) {
          setSortDirection("asc");
          return key;
        }
        setSortDirection((dir) => (dir === "asc" ? "desc" : "asc"));
        return current;
      });
    },
    []
  );

  const sortedStocks = useMemo(() => {
    const column = columns.find((col) => col.key === sortKey);
    const getValue = column?.sortValue;
    const direction = sortDirection === "asc" ? 1 : -1;
    const sorted = [...stocks];
    sorted.sort((a, b) => {
      const left = getValue ? getValue(a) : a[sortKey];
      const right = getValue ? getValue(b) : b[sortKey];
      if (left === right) return 0;
      if (left === null || left === undefined) return -1 * direction;
      if (right === null || right === undefined) return 1 * direction;
      if (typeof left === "string" && typeof right === "string") {
        return left.localeCompare(right) * direction;
      }
      return (left > right ? 1 : -1) * direction;
    });
    return sorted;
  }, [stocks, columns, sortKey, sortDirection]);

  return (
    <div className="page">
      <header className="header">
        <div>
          <p className="eyebrow">Live Stocks</p>
          <h1>Stock Ticker Dashboard</h1>
          <p className="subtitle">SignalR updates with 10% random API failures.</p>
        </div>
        <div className="status-card">
          <div>
            <span className={`status-dot ${status}`} />
            <span className="status-text">{status}</span>
          </div>
          <div className="actions">
            <button className="refresh" onClick={loadStocks}>
              Refresh
            </button>
          </div>
        </div>
      </header>

      {loading && <div className="notice">Loading stocks...</div>}
      {error && <div className="notice error">{error}</div>}

      <DataGrid
        columns={columns}
        rows={sortedStocks}
        sortKey={sortKey}
        sortDirection={sortDirection}
        onSort={handleSort}
      />
    </div>
  );
}

export default App;
