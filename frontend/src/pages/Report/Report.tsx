import { useEffect } from "react";

import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import { useUnit, useStoreMap } from "effector-react";
import { useParams, useNavigate } from "react-router-dom";

import { useReportPageSocket } from "@/hooks/useReportPageSocket";
import { useSocketEvent } from "@/hooks/useSocketEvent";
import { $allBugsStore } from "@/store";
import { createLocalBugEvent } from "@/store/localBugs";
import {
  $initialReportStore,
  $titleStore,
  $creatorUserNameStore,
  patchReportSocketEvent,
  changeTitleEvent,
  saveTitleEvent,
  updateReportPathIdEvent,
} from "@/store/report";
import { BugClientEntity } from "@/types/bug";
import { SocketEvent } from "@/webSocketApi/models";

import Bug from "./components/Bug/Bug";

const ReportPage = () => {
  const navigate = useNavigate();
  const { reportId } = useParams();
  const initialReport = useUnit($initialReportStore);
  const title = useUnit($titleStore);
  const creatorUserName = useUnit($creatorUserNameStore);

  const allBugs = useStoreMap({
    store: $allBugsStore,
    keys: [reportId],
    fn: ([bugsData, localBugs], [currentReportId]) => {
      if (!currentReportId) return [];

      const { bugs, reportBugs } = bugsData;
      const bugIds = reportBugs[Number(currentReportId)] || [];

      // 1) Собираем мапу bug.id → clientId для любого локального багa
      const clientIdMap: Record<number, number> = {};
      localBugs.forEach((bug) => {
        // у вас в store и локальный, и промоушн лежат, нам нужна обоих запись
        clientIdMap[bug.id] = bug.clientId;
      });

      // 2) Берём бекендовые баги и подставляем правильный clientId
      const bugsFromStore = bugIds
        .map((id: number) => bugs[id])
        .filter(Boolean)
        .map((bug) => ({
          ...bug,
          clientId: clientIdMap[bug.id] ?? bug.id, // ← вот здесь ключ сохраняется прежним
        }));

      // 3) К этим добавляем только ещё непромоушненные локальные
      const pendingLocals = localBugs.filter(
        (bug) => bug.reportId === Number(currentReportId) && bug.isLocalOnly
      );

      return [...bugsFromStore, ...pendingLocals];
    },
  });

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

  // редирект после создания репорта
  useEffect(() => {
    if (!reportId && initialReport?.id) {
      navigate(`/new-reports/${initialReport.id}`);
    }
  }, [initialReport?.id, reportId, navigate]);

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
        {allBugs.map((bug: BugClientEntity) => (
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
