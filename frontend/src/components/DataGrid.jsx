import React, { useMemo } from "react";
import { FixedSizeList as List } from "react-window";

const Row = React.memo(function Row({ index, style, data }) {
  const item = data.rows[index];
  return (
    <div
      className={`grid-row ${index % 2 === 0 ? "even" : "odd"}`}
      style={{ ...style, gridTemplateColumns: data.gridTemplateColumns }}
    >
      {data.columns.map((col) => (
        <div className="grid-cell" key={col.key}>
          {col.format ? col.format(item[col.key], item) : item[col.key]}
        </div>
      ))}
    </div>
  );
});

export default function DataGrid({
  columns,
  rows,
  height = 420,
  rowHeight = 44,
  sortKey,
  sortDirection,
  onSort
}) {
  const gridTemplateColumns = useMemo(
    () => columns.map((col) => col.width || "1fr").join(" "),
    [columns]
  );

  const listData = useMemo(
    () => ({ columns, rows, gridTemplateColumns }),
    [columns, rows, gridTemplateColumns]
  );

  if (!rows.length) {
    return <div className="grid-empty">No data available.</div>;
  }

  return (
    <div className="grid">
      <div className="grid-header" style={{ gridTemplateColumns }}>
        {columns.map((col) => {
          const isSorted = sortKey === col.key;
          const indicator = isSorted ? (sortDirection === "asc" ? "▲" : "▼") : "↕";
          return (
            <button
              type="button"
              className={`grid-header-cell sort-button ${isSorted ? "active" : ""}`}
              key={col.key}
              onClick={() => onSort?.(col.key)}
            >
              <span>{col.label}</span>
              <span className="sort-indicator">{indicator}</span>
            </button>
          );
        })}
      </div>
      <List
        height={height}
        itemCount={rows.length}
        itemSize={rowHeight}
        width="100%"
        itemData={listData}
      >
        {Row}
      </List>
    </div>
  );
}
