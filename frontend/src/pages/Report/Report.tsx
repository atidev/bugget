import { useEffect } from "react";
import { setBreadcrumbs } from "@/store/breadcrumbs";
import { useUnit } from "effector-react";
import { useNavigate, useParams } from "react-router-dom";
import {
  fetchReportFx,
  clearReport,
  createReportFx,
  $reportForm,
} from "@/store/report";
import { $newBugStore, $isExists, setExists } from "@/store/newBug";
import { $bugsIds } from "@/store/bugs";
import { getCommentsFx } from "@/store/comments";
import Bug from "./components/Bug/Bug";
import ReportHeader from "./components/ReportHeader/ReportHeader";
import "./Report.css";
import useWebSocketReportPage from "@/hooks/useWebSocketReportPage";
import { ReportStatuses } from "@/const";

const reportsPageBreadcrumb = { label: "Репорты", path: "/" };

const ReportPage = () => {
  const { reportId } = useParams();
  const navigate = useNavigate();
  const isNewReport = !reportId;
  const bugsIds = useUnit($bugsIds);

  useEffect(() => {
    setBreadcrumbs([
      reportsPageBreadcrumb,
      isNewReport
        ? { label: "Новый репорт", path: `/reports` }
        : {
            label: `Репорт #${reportId}`,
            path: `/reports/${reportId}`,
          },
    ]);
  }, [reportId, isNewReport]);

  const [reportForm, newBugStore, isExists] = useUnit([
    $reportForm,
    $newBugStore,
    $isExists,
  ]);

  // Загружаем отчет, если есть ID
  useEffect(() => {
    if (reportId) {
      fetchReportFx(Number(reportId));
    }

    return () => {
      clearReport();
    };
  }, [reportId]);

  const receiveReportHandler = (reportId: number) => {
    fetchReportFx(reportId);
  };

  const receiveCommentsHandler = (reportId: number, bugId: number) => {
    getCommentsFx({ reportId, bugId });
  };

  // Используем хук для работы с SignalR
  useWebSocketReportPage(
    Number(reportForm.id),
    receiveCommentsHandler,
    receiveReportHandler
  );

  const handleCreateReport = async () => {
    const responsibleId = reportForm.responsible?.id;
    if (!responsibleId) return;
    const newReport = await createReportFx({
      title: reportForm.title,
      responsibleId,
      status: ReportStatuses.IN_PROGRESS,
      bugs: [
        {
          receive: newBugStore.receive,
          expect: newBugStore.expect,
          isReady: false,
        },
      ],
    });
    if (newReport.id) {
      navigate(`/reports/${newReport.id}`);
    }
  };

  return (
    <div className="reports-wrapper">
      <ReportHeader />
      {bugsIds.length ? (
        bugsIds.map((id) => (
          <Bug key={id} reportId={reportForm.id} bugId={id} />
        ))
      ) : (
        <Bug />
      )}

      {!isNewReport && bugsIds.length > 0 && !isExists && (
        <button
          className="btn btn-block btn-outline btn-info mt-5"
          onClick={() => setExists(true)}
        >
          + Добавить баг
        </button>
      )}

      {isExists && <Bug reportId={reportForm.id} />}

      {isNewReport && newBugStore.isReady && reportForm.responsible?.id && (
        <div className="button-wrapper">
          <button
            onClick={handleCreateReport}
            className="btn btn-primary font-normal"
          >
            Создать репорт
          </button>
        </div>
      )}
    </div>
  );
};

export default ReportPage;
