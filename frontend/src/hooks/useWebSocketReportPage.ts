import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const useWebSocketReportPage = (
  reportId: number,
  onNewComment: (bugId: number) => void,
  onReportUpdate: (reportId: number) => void
) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const previousReportIdRef = useRef<number | null>(null);

  useEffect(() => {
    if (!reportId) {
      return;
    }

    if (!connectionRef.current) {
      console.log("🚀 Создаём новое соединение SignalR...");

      previousReportIdRef.current = reportId;

      connectionRef.current = new signalR.HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_BASE_URL}v1/report-page-hub`, {
          transport: signalR.HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Information)
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

      // 📌 Получаем баг, комменты которого нужно обновить
      connection.on("ReceiveComments", (bugId: number) => {
        console.log(`🔔 Новый комментарий для бага ${bugId}`);
        onNewComment(bugId);
      });

      // 📌 Обновление всего репорта
      connection.on("ReceiveReport", () => {
        console.log("🔄 Обновление репорта...");
        onReportUpdate(reportId);
      });

      // ⚠️ Обработчик разрыва соединения
      connection.onclose((error) => {
        console.error("⚠️ SignalR разорвал соединение:", error || "Нет ошибки");
        setTimeout(() => {
          if (connection.state === signalR.HubConnectionState.Disconnected) {
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
    if (connection?.state === signalR.HubConnectionState.Connected) {
      if (previousReportIdRef.current !== null) {
        console.log(`🚪 Выходим из группы: ${previousReportIdRef.current}`);
        connection
          .invoke("LeaveReportGroupAsync", previousReportIdRef.current)
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

      previousReportIdRef.current = reportId;
    }
  }, [reportId]);

  return {};
};

export default useWebSocketReportPage;
