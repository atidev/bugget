import { useUnit } from "effector-react";
import { useCallback, useEffect, useRef } from "react";
import { HubConnectionState } from "@microsoft/signalr";

import {
  $connection,
  connectionReconnected,
  joinReportFx,
  leaveReportFx,
} from "@/store/socket";
import { $initialReportStore } from "@/store/report";

// использование сокет соединения на странице репорта
export const useReportPageSocket = () => {
  const connection = useUnit($connection);
  const initialReport = useUnit($initialReportStore);
  const currentReportId = useRef<number | null>(null);

  const isConnected = connection?.state === HubConnectionState.Connected;

  const join = useCallback(
    (id: number) =>
      joinReportFx({ conn: connection!, reportId: id }).catch(console.error),
    [connection]
  );

  const leave = useCallback(
    (id: number) =>
      leaveReportFx({ conn: connection!, reportId: id }).catch(console.error),
    [connection]
  );

  // Повторное присоединение после восстановления соединения
  useEffect(() => {
    if (!connection) return;

    const stop = connectionReconnected.watch(() => {
      if (isConnected && currentReportId.current != null) {
        join(currentReportId.current);
      }
    });

    return stop;
  }, [connection, isConnected, join]);

  // Первичное присоединение, смена репорта, отключение
  useEffect(() => {
    if (!isConnected) return;

    const newId = initialReport?.id ?? null;
    if (newId === currentReportId.current) return;

    if (currentReportId.current != null) leave(currentReportId.current);

    if (newId != null) {
      currentReportId.current = newId;
      join(newId);
    }

    return () => {
      if (currentReportId.current != null) {
        leave(currentReportId.current);
        currentReportId.current = null;
      }
    };
  }, [isConnected, initialReport?.id, join, leave]);
};
