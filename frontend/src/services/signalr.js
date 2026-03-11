import * as signalR from "@microsoft/signalr";

const apiBaseUrl = process.env.REACT_APP_API_BASE_URL || "http://localhost:5000";

export function createInventoryConnection() {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hub/inventory`)
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();
}
