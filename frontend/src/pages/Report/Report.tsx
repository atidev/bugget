import { useState, useEffect } from "react";
import { useUnit, useStoreMap } from "effector-react";
import { useParams, useNavigate } from "react-router-dom";
import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";

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
import { $bugsData } from "@/store/bugs";
import { SocketEvent } from "@/webSocketApi/models";
import { BugEntity } from "@/types/bug";
import { BugStatuses } from "@/const";

import Bug from "./components/Bug/Bug";

const ReportPage = () => {
  const navigate = useNavigate();
  const { reportId } = useParams();
  const initialReport = useUnit($initialReportStore);
  const title = useUnit($titleStore);
  const creatorUserName = useUnit($creatorUserNameStore);

  // баги существующие только на фронте
  const [onlyUIBugs, setOnlyUIBugs] = useState<BugEntity[]>([]);

  const bugsFromStore = useStoreMap({
    store: $bugsData,
    keys: [reportId],
    fn: ({ bugs, reportBugs }, [currentReportId]) => {
      if (!currentReportId) return [];
      const bugIds = reportBugs[Number(currentReportId)] || [];
      return bugIds.map((id) => bugs[id]).filter(Boolean);
    },
  });

  const allBugs = [...bugsFromStore, ...onlyUIBugs];

  useReportPageSocket();
  useSocketEvent(SocketEvent.ReportPatch, (patch) =>
    patchReportSocketEvent(patch)
  );

  const handleRemoveTemporaryBug = (bugId: number) => {
    // удаляем с ui фронтовый баг и обновляем тем, что с бэкенда
    setOnlyUIBugs((prev) => prev.filter((bug) => bug.id !== bugId));
  };

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

  const handleAddBugClick = () => {
    if (!reportId) return;

    const currentISODate = new Date().toISOString();

    // Создаем локальный новый баг на фронте
    const newLocalBug: BugEntity = {
      id: Date.now(),
      receive: "",
      expect: "",
      status: BugStatuses.ACTIVE,
      createdAt: currentISODate,
      updatedAt: currentISODate,
      creatorUserId: "",
      reportId: Number(reportId),
      attachments: null,
      comments: null,
      isLocalOnly: true,
    };

    setOnlyUIBugs((prev) => [...prev, newLocalBug]);
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
        {allBugs?.map((bug: BugEntity) => (
          <Bug
            key={bug.id}
            bug={bug}
            onRemoveTemporary={
              bug.isLocalOnly ? handleRemoveTemporaryBug : undefined
            }
          />
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
