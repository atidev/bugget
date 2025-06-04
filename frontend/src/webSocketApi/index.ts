import {
  HubConnection,
  HubConnectionBuilder,
  HttpTransportType,
  LogLevel,
} from "@microsoft/signalr";
import { backoff } from "@/utils/retry";

const API_URL = `${import.meta.env.VITE_BASE_URL}v1/report-page-hub`;
const RECONNECT_DELAYS = [0, 2_000, 5_000, 10_000, 30_000, 60_000, 120_000]; // до 2 мин

export const buildConnection = (): HubConnection => {
  const conn = new HubConnectionBuilder()
    .withUrl(API_URL, {
      transport: HttpTransportType.WebSockets
    })
    .withAutomaticReconnect(RECONNECT_DELAYS)
    .configureLogging(
      import.meta.env.DEV ? LogLevel.Information : LogLevel.Error,
    )
    .build();

  conn.serverTimeoutInMilliseconds = 15_000;
  conn.keepAliveIntervalInMilliseconds = 5_000;

  return conn;
};

export const startConnection = async (conn: HubConnection) => {
  await backoff(() => conn.start());
};