import { ReportStatuses } from "@/const";
import { useReportPageSocket } from "@/hooks/useReportPageSocket";
import { useSocketEvent } from "@/hooks/useSocketEvent";
import {
    $initialReportStore,
    $titleStore,
    $statusStore,
    $responsibleUserIdStore,
    $pastResponsibleUserIdStore,
    $creatorUserIdStore,
    $updatedAtStore,
    patchReportSocketEvent,
    changeTitleEvent,
    saveTitleEvent,
    changeStatusEvent,
    changeResponsibleUserIdEvent,
    updateReportPathIdEvent
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
    const status = useUnit($statusStore);
    const responsibleUserId = useUnit($responsibleUserIdStore);
    const pastResponsibleUserId = useUnit($pastResponsibleUserIdStore);
    const creatorUserId = useUnit($creatorUserIdStore);
    const updatedAt = useUnit($updatedAtStore);

    useReportPageSocket();
    useSocketEvent(SocketEvent.ReportPatch, (patch) =>
        patchReportSocketEvent(patch)
    );

    // состояние страницы
    useEffect(() => {
        if (reportId) {
            updateReportPathIdEvent(parseInt(reportId));
        }
        else {
            updateReportPathIdEvent(null);
        }
    }, [reportId]);

    // редирект после создания репорта
    useEffect(() => {
        if (!reportId && initialReport?.id) {
            navigate(`/new-reports/${initialReport.id}`);
        }
    }, [initialReport?.id, reportId]);

    return (
        <div className="flex flex-col gap-2">
            <div className="flex flex-row gap-2">
                <p className="font-bold">ID:</p>
                <p>{initialReport?.id ?? "New Report"}</p>
            </div>
            <input
                type="text"
                value={title}
                onChange={(e) => changeTitleEvent(e.target.value)}
                onBlur={() => saveTitleEvent()}
                placeholder="Заголовок репорта"
            />
            <div className="flex flex-row gap-2">
                <p className="font-bold">Статус:</p>
                <select
                    value={status}
                    onChange={(e) => changeStatusEvent(Number(e.target.value) as ReportStatuses)}
                >
                    <option value={ReportStatuses.BACKLOG}>Backlog</option>
                    <option value={ReportStatuses.RESOLVED}>Resolved</option>
                    <option value={ReportStatuses.IN_PROGRESS}>In Progress</option>
                    <option value={ReportStatuses.REJECTED}>Rejected</option>
                </select>
            </div>
            <div className="flex flex-row gap-2">
                <p className="font-bold">Ответственный:</p>
                <input type="text" value={responsibleUserId} onChange={(e) => changeResponsibleUserIdEvent(e.target.value)} />
            </div>
            <div className="flex flex-row gap-2">
                <p className="font-bold">Предыдущий ответственный (показывать не будем, нужно будет для логики):</p>
                <p>{pastResponsibleUserId}</p>
            </div>
            <div className="flex flex-row gap-2">
                <p className="font-bold">Создатель:</p>
                <p>{creatorUserId}</p>
            </div>
            <div className="flex flex-row gap-2">
                <p className="font-bold">Дата создания:</p>
                <p>{initialReport?.createdAt ?? "тут будет дата создания"}</p>
            </div>
            <div className="flex flex-row gap-2">
                <p className="font-bold">Дата обновления:</p>
                <p>{updatedAt}</p>
            </div>
        </div>
    );
};

export default ReportPage;