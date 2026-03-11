import axios from "axios";

const apiBaseUrl = "http://localhost:5000";

export const apiClient = axios.create({
  baseURL: `${apiBaseUrl}/api`,
  timeout: 8000
});

export async function fetchStocks() {
  const response = await apiClient.get("/stocks");
  return response.data;
}
