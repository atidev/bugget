import { useEffect } from "react";
import { setBreadcrumbs } from "@/store/breadcrumbs";
import { useList, useUnit, createGate, useGate } from "effector-react";
import { useNavigate, useParams } from "react-router-dom";
import {
  fetchReportFx,
  clearReport,
  createReportFx,
  $reportForm,
} from "@/store/report";
import { $newBugStore, setExists, $isExists } from "@/store/newBug";
import { $bugsIds } from "@/store/bugs";
import { getCommentsFx } from "@/store/comments";
import Bug from "./components/Bug/Bug";
import ReportHeader from "./components/ReportHeader/ReportHeader";
import "./Report.css";
import useWebSocketReportPage from "@/hooks/useWebSocketReportPage";
import { ReportStatuses } from "@/const";
// import Skeleton from "./components/Skeleton/Skeleton";

const reportsPageBreadcrumb = { label: "Репорты", path: "/" };
const SampleCompGate = createGate();

const ReportPage = () => {
  const { reportId } = useParams();
  const navigate = useNavigate();

  useGate(SampleCompGate); // instead of use Effect
  const isLoading = useUnit(fetchReportFx.pending);

  const isNewReport = !reportId;

  const [reportForm, newBugStore, setExistsHandler, isExists, getComments] =
    useUnit([$reportForm, $newBugStore, setExists, $isExists, getCommentsFx]);

  const bugsIds = useUnit($bugsIds);
  const bugsList = useList($bugsIds, (id) => (
    <Bug key={id} reportId={reportForm.id} bugId={id} isLoading={isLoading} />
  ));

  const receiveReportHandler = (reportId: number) => {
    fetchReportFx(reportId);
  };

  const receiveCommentsHandler = (reportId: number, bugId: number) => {
    getComments({ reportId, bugId });
  };

  // Используем хук для работы с SignalR
  useWebSocketReportPage(
    Number(reportForm.id),
    receiveCommentsHandler,
    receiveReportHandler
  );

  // Загружаем отчет, если есть ID
  useEffect(() => {
    if (reportId) {
      fetchReportFx(Number(reportId));
    }

    return () => {
      clearReport();
    };
  }, [reportId]);

  useEffect(() => {
    setBreadcrumbs(
      [
        reportsPageBreadcrumb,
        isNewReport
          ? { label: "Новый репорт", path: `/reports` }
          : {
              label: `Репорт #${reportId}`,
              path: `/reports/${reportId}`,
            },
      ].filter(Boolean)
    );
  }, [reportId, isNewReport]);

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
    if (newReport?.id) {
      navigate(`/reports/${newReport.id}`);
    }
  };

  return (
    <div className="reports-wrapper">
      <ReportHeader isLoading={isLoading} />
      {bugsIds.length > 0 ? bugsList : <Bug isLoading={isLoading} />}

      {!isNewReport && bugsIds.length > 0 && !isExists && (
        <button
          className="btn btn-block btn-outline btn-info mt-5"
          onClick={() => setExistsHandler(true)}
        >
          + Добавить баг
        </button>
      )}

      {isExists && <Bug reportId={reportForm.id} isLoading={isLoading} />}

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
