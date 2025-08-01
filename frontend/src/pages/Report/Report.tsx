import { useReportPageSocket } from "@/hooks/useReportPageSocket";
import { useSocketEvent } from "@/hooks/useSocketEvent";
import {
  $initialReportStore,
  $titleStore,
  $creatorUserNameStore,
  patchReportSocketEvent,
  changeTitleEvent,
  saveTitleEvent,
  updateReportPathIdEvent,
} from "@/store/report";
import { SocketEvent } from "@/webSocketApi/models";
import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import { useUnit, useStoreMap } from "effector-react";
import { useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Bug from "./components/Bug/Bug";
import { $bugsData } from "@/store/bugs";
import { BugEntity } from "@/types/bug";

function sortBugsByDate(a: BugEntity, b: BugEntity) {
  return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
}

const ReportPage = () => {
  const navigate = useNavigate();
  const { reportId } = useParams();
  const initialReport = useUnit($initialReportStore);
  const title = useUnit($titleStore);
  const creatorUserName = useUnit($creatorUserNameStore);

  const bugs = useStoreMap({
    store: $bugsData,
    keys: [reportId],
    fn: ({ bugs, reportBugs }, [currentReportId]) => {
      if (!currentReportId) return [];
      const bugIds = reportBugs[Number(currentReportId)] || [];
      return bugIds
        .map((id) => bugs[id])
        .filter(Boolean)
        .sort(sortBugsByDate);
    },
  });

  useReportPageSocket();
  useSocketEvent(SocketEvent.ReportPatch, (patch) =>
    patchReportSocketEvent(patch)
  );

  // состояние страницы
  useEffect(() => {
    if (reportId) {
      updateReportPathIdEvent(parseInt(reportId));
    } else {
      updateReportPathIdEvent(null);
    }
  }, [reportId]);

  // редирект после создания репорта
  useEffect(() => {
    if (!reportId && initialReport?.id) {
      navigate(`/new-reports/${initialReport.id}`);
    }
  }, [initialReport?.id, reportId, navigate]);

  return (
    <>
      <input
        type="text"
        value={title}
        onChange={(e) => changeTitleEvent(e.target.value)}
        onBlur={() => saveTitleEvent()}
        placeholder="Заголовок репорта"
        className="input-lg text-2xl"
      />
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
      <div className="flex flex-col gap-2">
        {bugs?.map((bug) => (
          <Bug key={bug.id} bug={bug} />
        ))}
      </div>
    </>
  );
};

export default ReportPage;
