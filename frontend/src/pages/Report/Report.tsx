import { useReportPageSocket } from "@/hooks/useReportPageSocket";
import { useSocketEvent } from "@/hooks/useSocketEvent";
import {
  $initialReportStore,
  $titleStore,
  $creatorUserIdStore,
  patchReportSocketEvent,
  changeTitleEvent,
  saveTitleEvent,
  updateReportPathIdEvent,
} from "@/store/report";
import { SocketEvent } from "@/webSocketApi/models";
import { useUnit } from "effector-react";
import { useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

const ReportPage = () => {
  const navigate = useNavigate();
  const { reportId } = useParams();
  const initialReport = useUnit($initialReportStore);
  const title = useUnit($titleStore);
  const creatorUserId = useUnit($creatorUserIdStore);

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
      <div className="flex flex-col gap-4 px-6 py-8">
        {/* <div className="flex flex-row gap-2">
                    <p className="font-bold">ID:</p>
                    <p>{initialReport?.id ?? "New Report"}</p>
                </div> */}
        <input
          type="text"
          value={title}
          onChange={(e) => changeTitleEvent(e.target.value)}
          onBlur={() => saveTitleEvent()}
          placeholder="Заголовок репорта"
          className="input-lg text-2xl"
        />
        <div>
          Создан {initialReport?.createdAt} пользователем{" "}
          <strong>{creatorUserId}</strong>
        </div>
        <div className="flex flex-col gap-2">
          {/* баг */}
          <div className="grid grid-cols-2"></div>
        </div>
      </div>
    </>
  );
};

export default ReportPage;
