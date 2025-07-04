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
    // todo: переделать на универсальную обработку массива аргументов в событии
    if (event === SocketEvent.BugPatch) {
      // Специальная обработка для ReceiveBugPatch - получаем bugId и patch отдельно
      const handler = (...args: unknown[]) => {
        const bugId = args[0] as number;
        const patch = args[1] as {
          receive?: string;
          expect?: string;
          status?: number;
        };
        console.log(`🔄 [Socket] Received event:`, event, { bugId, patch });
        socketEventReceived({ type: event, payload: { bugId, patch } } as {
          type: SocketEvent;
          payload: {
            bugId: number;
            patch: { receive?: string; expect?: string; status?: number };
          };
        });
      };
      conn.on(event, handler);
      handlers.set(event, handler as (p: unknown) => void);
    } else {
      // Обычная обработка для событий с одним параметром
      const handler = (payload: unknown) => {
        console.log(`🔄 [Socket] Received event:`, event, payload);
        socketEventReceived({ type: event, payload } as {
          type: SocketEvent;
          payload: SocketPayload[SocketEvent];
        });
      };
      conn.on(event, handler);
      handlers.set(event, handler);
    }
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
