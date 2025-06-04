import { createDomain } from "effector";
import { buildConnection, startConnection } from "@/webSocketApi";
import { HubConnection } from "@microsoft/signalr";
import { SocketEvent, SocketPayload } from "@/webSocketApi/models";
import { setSignalRConnectionId } from "@/api/axios";

type ConnectionReady = HubConnection & { started: true };

const socket = createDomain();

export const connectionStarted = socket.createEvent<ConnectionReady>();
export const connectionClosed = socket.createEvent<Error | undefined>();
export const connectionReconnected = socket.createEvent();

export const socketEventReceived = socket.createEvent<{
  type: SocketEvent;
  payload: SocketPayload[SocketEvent];
}>();

export const joinReportFx = socket.createEffect(
  async ({ conn, reportId }: { conn: HubConnection; reportId: number }) => {
    await conn.invoke("JoinReportGroupAsync", reportId);
  }
);

export const leaveReportFx = socket.createEffect(
  async ({ conn, reportId }: { conn: HubConnection; reportId: number }) => {
    await conn.invoke("LeaveReportGroupAsync", reportId);
  }
);

export const $connection = socket
  .createStore<ConnectionReady | null>(null)
  .on(connectionStarted, (_, conn) => {
    setSignalRConnectionId(conn.connectionId ?? null);
    return conn;
  })
  .reset(connectionClosed);

export const $isConnected = $connection.map(Boolean);

export const initSocketFx = socket.createEffect(async () => {
  const conn = buildConnection();

  /** Карта handlers, нужна чтобы затем корректно вызвать `conn.off` */
  const handlers = new Map<SocketEvent, (p: unknown) => void>();

  // регистрируем единый набор хендлеров
  Object.values(SocketEvent).forEach((event) => {
    const handler = (payload: unknown) => {
      socketEventReceived({ type: event, payload } as any);
    };
    conn.on(event, handler);
    handlers.set(event, handler);
  });

  // системные события соединения
  conn.onreconnected(() => {
    connectionReconnected();
    const conn = $connection.getState();
    setSignalRConnectionId(conn?.connectionId ?? null);
  });

  conn.onclose((e) => {
    handlers.forEach((h, ev) => conn.off(ev, h)); // clean-up
    connectionClosed(e);
    setSignalRConnectionId(null);
  });

  try {
    await startConnection(conn);
    connectionStarted(
      Object.assign(conn, { started: true }) as ConnectionReady
    );
  } catch (e) {
    console.error(e);
    connectionClosed(e as Error);
  }
});
