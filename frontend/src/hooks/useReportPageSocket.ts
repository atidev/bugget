import { $connection, initSocketFx, joinReportFx, leaveReportFx, connectionStarted } from "@/store/socket";
import { $initialReportStore } from "@/store/report";
import { useUnit } from "effector-react";
import { useEffect, useRef } from "react";
import { HubConnectionState } from "@microsoft/signalr";

export const useReportPageSocket = () => {
    const connection = useUnit($connection);
    const initialReport = useUnit($initialReportStore);
    const reportIdRef = useRef<number | null>(null);

    // Инициализируем сокет если есть репорт
    useEffect(() => {
        console.log("🚀 [Socket] Check init:", { 
            hasReport: !!initialReport?.id, 
            hasConnection: !!connection,
            currentReportId: initialReport?.id,
            prevReportId: reportIdRef.current
        });

        if (initialReport?.id && !connection) {
            console.log("🚀 [Socket] Initializing connection");
            initSocketFx().then(conn => {
                console.log("✅ [Socket] Connection started");
                connectionStarted(conn);

                // Обработчик разрыва соединения
                conn.onclose((error) => {
                    console.error("⚠️ [Socket] Connection closed:", error || "No error");
                    setTimeout(() => {
                        if (conn.state === HubConnectionState.Disconnected) {
                            console.log("🔄 [Socket] Trying to reconnect...");
                            conn.start()
                                .then(() => {
                                    console.log("✅ [Socket] Reconnected!");
                                    connectionStarted(conn);
                                    // Переподключаемся к текущему репорту
                                    if (reportIdRef.current) {
                                        joinReportFx({ conn, reportId: reportIdRef.current });
                                    }
                                })
                                .catch(err => console.error("❌ [Socket] Reconnection failed:", err));
                        }
                    }, 5000);
                });
            });
        }
    }, [initialReport?.id, connection]);

    // Подключаемся к репорту когда сокет готов
    useEffect(() => {
        if (!connection || !initialReport?.id) {
            console.log("⏭️ [Socket] Skip join:", { 
                hasConnection: !!connection, 
                hasReport: !!initialReport?.id,
                connectionState: connection?.state
            });
            return;
        }

        const currentReportId = initialReport.id;
        
        // Если уже подключены к этому репорту - пропускаем
        if (reportIdRef.current === currentReportId) {
            console.log("ℹ️ [Socket] Already connected to report:", currentReportId);
            return;
        }

        // Проверяем состояние соединения
        if (connection.state !== HubConnectionState.Connected) {
            console.log("⏳ [Socket] Waiting for connection...");
            return;
        }

        // Если были подключены к другому репорту - отключаемся
        if (reportIdRef.current !== null) {
            console.log("🚪 [Socket] Leaving previous report:", reportIdRef.current);
            void leaveReportFx({ conn: connection, reportId: reportIdRef.current });
        }

        console.log("🔄 [Socket] Joining report:", currentReportId);
        reportIdRef.current = currentReportId;
        joinReportFx({ conn: connection, reportId: currentReportId })
            .then(() => console.log("✅ [Socket] Joined report:", currentReportId))
            .catch(err => console.error("❌ [Socket] Failed to join report:", err));

        return () => {
            if (reportIdRef.current) {
                console.log("🧹 [Socket] Cleanup - leaving report:", reportIdRef.current);
                void leaveReportFx({ conn: connection, reportId: reportIdRef.current });
                reportIdRef.current = null;
            }
        };
    }, [connection, initialReport?.id]);
};