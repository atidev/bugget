import { useEffect, useMemo, useCallback } from "react";
import { useUnit, useList } from "effector-react";
import { useNavigate, useParams } from "react-router-dom";
import {
  fetchReportFx, clearReport, createReportFx, $reportForm
} from "@/store/report";
import { $newBugStore, setExists, $isExists } from "@/store/newBug";
import { $bugsIds } from "@/store/bugs";
import { getCommentsFx } from "@/store/comments";
import { setBreadcrumbs } from "@/store/breadcrumbs";
import Bug from "./components/Bug/Bug";
import ReportHeader from "./components/ReportHeader/ReportHeader";
import useWebSocketReportPage from "@/hooks/useWebSocketReportPage";
import "./Report.css";

const reportsPageBreadcrumb = { label: "Репорты", path: "/" };

const ReportPage = () => {
  const { reportId } = useParams();
  const navigate = useNavigate();

  const isNewReport = !reportId;
  const bugsList = useList($bugsIds, (id) =>
    reportForm.id ? <Bug key={id} reportId={reportForm.id} bugId={id} /> : null
  );

  const [reportForm, newBugStore, setExistsHandler, isExists, getComments] =
    useUnit([$reportForm, $newBugStore, setExists, $isExists, getCommentsFx]);
  const bugsIds = useUnit($bugsIds);

  // Формируем хлебные крошки
  const breadcrumbs = useMemo(() => [
    reportsPageBreadcrumb,
    isNewReport
      ? { label: "Новый репорт", path: "/reports" }
      : { label: `Репорт #${reportId}`, path: `/reports/${reportId}` }
  ], [reportId, isNewReport]);

  useEffect(() => {
    setBreadcrumbs(breadcrumbs);
  }, [breadcrumbs]);

  // Загружаем отчет при наличии `reportId`
  useEffect(() => {
    if (reportId) {
      fetchReportFx(Number(reportId));
    }
    return clearReport;
  }, [reportId]);

  // Подключение к WebSocket
  useWebSocketReportPage(
    Number(reportForm.id),
    (bugId) => getComments({ reportId: Number(reportId), bugId }),
    (reportId) => fetchReportFx(reportId)
  );

  const handleCreateReport = useCallback(async () => {
    const newReport = await createReportFx({
      title: reportForm.title,
      responsibleId: reportForm.responsible?.id || "",
      bugs: [{ receive: newBugStore.receive, expect: newBugStore.expect }]
    });
    if (newReport?.id) {
      navigate(`/reports/${newReport.id}`);
    }
  }, [reportForm, newBugStore, navigate]);

  { console.log("reportForm.id", reportForm.id); }

  console.log("isExists", isExists);


  return (
    <div className="reports-wrapper">
      <ReportHeader />
      {bugsIds.length ? bugsList : <Bug reportId={null} />}
      {!isNewReport && !isExists && (
        <button className="btn btn-block btn-outline btn-info mt-5" onClick={() => setExistsHandler(true)}>
          + Добавить баг
        </button>
      )}
      {isExists && <Bug reportId={reportForm.id} />}

      {isNewReport && newBugStore.isReady && (
        <div className="button-wrapper">
          <button onClick={handleCreateReport} className="btn btn-info text-white px-4 py-2 rounded">
            Создать репорт
          </button>
        </div>
      )}
    </div>
  );
};

export default ReportPage;