import { useUnit } from "effector-react";
import { useEffect, useRef } from "react";
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
  const reportIdRef = useRef<number | null>(null);

  useEffect(() => {
    if (!connection) return;
    const unsub = connectionReconnected.watch(() => {
      if (connection.state === HubConnectionState.Connected && reportIdRef.current) {
        joinReportFx({ conn: connection, reportId: reportIdRef.current });
      }
    });
    return () => unsub();
  }, [connection]);

  useEffect(() => {
    if (!connection || connection.state !== HubConnectionState.Connected) return;
    const currentId = initialReport?.id;
    if (currentId == null) return;

    if (reportIdRef.current === currentId) return;

    if (reportIdRef.current !== null) {
      leaveReportFx({ conn: connection, reportId: reportIdRef.current }).catch(console.error);
    }

    reportIdRef.current = currentId;
    joinReportFx({ conn: connection, reportId: currentId }).catch(console.error);

    // cleanup при размонтировании страницы
    return () => {
      if (reportIdRef.current !== null) {
        leaveReportFx({ conn: connection, reportId: reportIdRef.current }).catch(console.error);
        reportIdRef.current = null;
      }
    };
  }, [connection, initialReport?.id]);
};