import {
    HubConnectionBuilder,
    LogLevel,
    HttpTransportType,
    HubConnection,
  } from "@microsoft/signalr";
  import { backoff } from "@/utils/retry";
  
  const API_URL = import.meta.env.VITE_BASE_URL + "v1/report-page-hub";
  
  export const buildConnection = (): HubConnection =>
    new HubConnectionBuilder()
      .withUrl(API_URL, { transport: HttpTransportType.WebSockets })
      .withAutomaticReconnect([0, 2e3, 5e3, 1e4, 3e4])
      .configureLogging(LogLevel.Information)
      .build();
  
  export const startConnection = async (conn: HubConnection) => {
    await backoff(() => conn.start());
  };