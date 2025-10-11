import { useEffect } from "react";

import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import { useUnit } from "effector-react";
import { useParams, useNavigate, useLocation } from "react-router-dom";

import { useReportPageSocket } from "@/hooks/useReportPageSocket";
import { useSocketEvent } from "@/hooks/useSocketEvent";
import { $combinedBugsStore } from "@/store";
import { createLocalBugEvent } from "@/store/localBugs";
import { clearBugsEvent } from "@/store/bugs";
import {
  $initialReportStore,
  $titleStore,
  $creatorUserNameStore,
  patchReportSocketEvent,
  changeTitleEvent,
  saveTitleEvent,
  updateReportPathIdEvent,
  clearReport,
} from "@/store/report";
import { SocketEvent } from "@/webSocketApi/models";

import Bug from "./components/Bug/Bug";

const ReportPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { reportId } = useParams();
  const initialReport = useUnit($initialReportStore);
  const title = useUnit($titleStore);
  const creatorUserName = useUnit($creatorUserNameStore);
  const allBugs = useUnit($combinedBugsStore);

  useReportPageSocket();
  useSocketEvent(SocketEvent.ReportPatch, (patch) =>
    patchReportSocketEvent(patch)
  );

  // состояние страницы
  useEffect(() => {
    if (reportId) {
      updateReportPathIdEvent(Number(reportId));
    } else {
      updateReportPathIdEvent(null);
    }
  }, [reportId]);

  useEffect(() => {
    // Очищаем стейт при изменении reportId
    clearReport();
    clearBugsEvent();

    return () => {
      // Также очищаем при размонтировании
      clearReport();
      clearBugsEvent();
    };
  }, [reportId]);

  // редирект после создания репорта
  useEffect(() => {
    if (!reportId && initialReport?.id) {
      navigate(`${location.pathname}/${initialReport.id}`);
    }
  }, [initialReport?.id, reportId, navigate, location.pathname]);

  const handleAddBugClick = () => {
    createLocalBugEvent({ reportId: Number(reportId) });
  };

  return (
    <>
      <input
        type="text"
        value={title}
        onChange={(e) => changeTitleEvent(e.target.value)}
        onBlur={() => saveTitleEvent()}
        placeholder="Заголовок репорта"
        className="input-lg text-2xl"
        autoFocus={!reportId}
      />
      {reportId && (
        <div>
          Создан{" "}
          {initialReport?.createdAt
            ? formatDistanceToNow(new Date(initialReport.createdAt), {
                addSuffix: true,
                locale: ru,
              })
            : ""}{" "}
          пользователем <strong>{creatorUserName || "Загрузка..."}</strong>
        </div>
      )}
      <div className="flex flex-col gap-2">
        {allBugs.map((bug) => (
          <Bug key={bug.clientId} bug={bug} />
        ))}
        <button
          className="btn btn-outline btn-primary font-normal ml-auto"
          onClick={handleAddBugClick}
        >
          Добавить баг
        </button>
      </div>
    </>
  );
};

export default ReportPage;
