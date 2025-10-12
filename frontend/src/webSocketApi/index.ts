import {
  HubConnection,
  HubConnectionBuilder,
  HttpTransportType,
  LogLevel,
} from "@microsoft/signalr";
import { backoff } from "@/utils/retry";
import { API_URL } from "@/const";
import { extendedBasePath } from "@/api/basePath";

const RECONNECT_DELAYS = [0, 2_000, 5_000, 10_000, 30_000, 60_000, 120_000]; // до 2 мин

export const buildConnection = (): HubConnection => {
  // Строим URL для WebSocket с учетом extendedBasePath
  const baseUrl = API_URL.endsWith("/") ? API_URL.slice(0, -1) : API_URL;
  const hubPath = "v1/report-page-hub";
  const prefix = extendedBasePath || "";

  // Собираем URL: baseUrl + prefix + hubPath
  const fullUrl = `${baseUrl}${prefix}/${hubPath}`;

  console.log("fullUrl", fullUrl);

  const conn = new HubConnectionBuilder()
    .withUrl(fullUrl, {
      transport: HttpTransportType.WebSockets,
    })
    .withAutomaticReconnect(RECONNECT_DELAYS)
    .configureLogging(
      import.meta.env.DEV ? LogLevel.Information : LogLevel.Error
    )
    .build();

  conn.serverTimeoutInMilliseconds = 15_000;
  conn.keepAliveIntervalInMilliseconds = 5_000;

  return conn;
};

export const startConnection = async (conn: HubConnection) => {
  await backoff(() => conn.start());
};
