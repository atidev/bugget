import { useEffect, useRef } from "react";
import {
  HubConnectionBuilder,
  HttpTransportType,
  LogLevel,
  HubConnectionState,
} from "@microsoft/signalr";

const API_URL = window.env?.API_URL || import.meta.env.VITE_BASE_URL;

const useWebSocketReportPage = (
  reportId: number,
  onNewComment: (reportId: number, bugId: number) => void,
  onReportUpdate: (reportId: number) => void
) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const reportIdRef = useRef<number | null>(null);

  useEffect(() => {
    if (!reportId) {
      return;
    }

    const previousReportId = reportIdRef.current;
    if (!connectionRef.current) {
      console.log("🚀 Создаём новое соединение SignalR...");

      connectionRef.current = new HubConnectionBuilder()
        .withUrl(`${API_URL}v1/report-page-hub`, {
          transport: HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(LogLevel.Information)
        .build();

      const connection = connectionRef.current;

      connection
        .start()
        .then(() => {
          console.log("✅ SignalR Connected!");
          return connection.invoke("JoinReportGroupAsync", reportId);
        })
        .then(() => console.log(`✅ Присоединились к группе: ${reportId}`))
        .catch((err) => console.error("❌ Ошибка подключения SignalR:", err));

      reportIdRef.current = reportId;

      // 📌 Получаем баг, комменты которого нужно обновить
      connection.on("ReceiveComments", (bugId: number) => {
        console.log(
          `🔔 Новый комментарий для бага ${bugId}, репорта ${reportIdRef.current}`
        );
        if (reportIdRef.current) onNewComment(reportIdRef.current, bugId);
      });

      // 📌 Обновление всего репорта
      connection.on("ReceiveReport", () => {
        console.log("🔄 Обновление репорта...", reportIdRef.current);
        if (reportIdRef.current) onReportUpdate(reportIdRef.current);
      });

      // ⚠️ Обработчик разрыва соединения
      connection.onclose((error) => {
        console.error("⚠️ SignalR разорвал соединение:", error || "Нет ошибки");
        setTimeout(() => {
          if (connection.state === HubConnectionState.Disconnected) {
            connection
              .start()
              .catch((err) =>
                console.error("🚨 Ошибка при переподключении:", err)
              );
          }
        }, 5000);
      });
    }

    const connection = connectionRef.current;
    if (connection?.state === HubConnectionState.Connected) {
      if (previousReportId !== null) {
        console.log(`🚪 Выходим из группы: ${previousReportId}`);
        connection
          .invoke("LeaveReportGroupAsync", previousReportId)
          .catch((err) =>
            console.error("❌ Ошибка при выходе из группы:", err)
          );
      }

      console.log(`🔄 Подключаемся к новой группе: ${reportId}`);
      connection
        .invoke("JoinReportGroupAsync", reportId)
        .then(() => console.log(`✅ Переключились на группу: ${reportId}`))
        .catch((err) =>
          console.error("❌ Ошибка при смене группы SignalR:", err)
        );

      reportIdRef.current = reportId;
    }
  }, [reportId]);

  return {};
};

export default useWebSocketReportPage;
