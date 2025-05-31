import { createDomain, sample } from "effector";
import { buildConnection, startConnection } from "@/webSocketApi";
import { SocketEvent, SocketPayload } from "@/webSocketApi/models";
import { HubConnection } from "@microsoft/signalr";
import { SocketSubscription } from "@/types/socket";
import { $reportIdStore } from "./report";

const socket = createDomain();
const init = socket.createEvent();

export const joinReportFx = socket.effect(
    async ({ conn, reportId }: { conn: HubConnection; reportId: number }) => {
      await conn.invoke("JoinReportGroupAsync", reportId);
    }
  );
  
export const leaveReportFx = socket.effect(
    async ({ conn, reportId }: { conn: HubConnection; reportId: number }) => {
      await conn.invoke("LeaveReportGroupAsync", reportId);
    }
  );

export const $connection = socket.store<HubConnection | null>(null);
export const $isConnected = $connection.map(Boolean);

export const connectionStarted = socket.event<HubConnection>();
export const connectionFailed = socket.event<Error>();

export const socketEventReceived =
  socket.event<SocketSubscription<SocketEvent>>();

export const initSocketFx = socket.effect(async () => {
  const conn = buildConnection();
  conn.onclose(() => connectionFailed(new Error("closed")));
  
  // Регистрируем обработчики событий
  Object.values(SocketEvent).forEach(event => {
    conn.on(event, (payload: SocketPayload[typeof event]) => {
      console.log(`📡 [Socket] Received event: ${event}`, payload);
      socketEventReceived({ type: event, payload });
    });
  });

  await startConnection(conn);
  return conn;
});

export const subscribeFx = socket.effect(
    async <E extends SocketEvent>({
        conn,
        event,
    }: {
        conn: HubConnection;
        event: E;
    }) => {
        conn.on(event, (payload: SocketPayload[E]) =>
            socketEventReceived({ type: event, payload })
        );
    }
);

$connection.on(connectionStarted, (_, c) => c);

sample({
  clock: $reportIdStore,
  filter: (reportId): reportId is number => reportId !== null,
  target: initSocketFx,
});

init();

connectionFailed.watch((e) => console.error("socket error", e));