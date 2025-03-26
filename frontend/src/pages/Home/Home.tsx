import { useEffect } from "react";
import { useUnit } from "effector-react";
import {
  loadReportsFx,
  $responsibleReports,
  $participantReports,
} from "../../store/reportsSummary";
import ReportSummary from "./components/ReportSummary/ReportSummary";
import "./Home.css";

const Main = () => {
  const responsibleReports = useUnit($responsibleReports);
  const participantReports = useUnit($participantReports);

  useEffect(() => {
    loadReportsFx();
  }, []);

  return (
    <div className="text-base-content auto-rows-min gap-4">
      <section className="flex flex-column section">
        <h2 className="section-title text-base-content">Ответственный</h2>
        <div className="reports-summary-wrapper">
          {!!responsibleReports.length &&
            responsibleReports.map((report) => (
              <ReportSummary key={report.id} report={report} highlight />
            ))}
        </div>
      </section>
      <div className="divider section-divider"></div>
      <section className="flex flex-column section">
        <h2 className="section-title text-base-content">Участник</h2>
        <div className="reports-summary-wrapper">
          {!!participantReports.length &&
            participantReports.map((report) => (
              <ReportSummary key={report.id} report={report} />
            ))}
        </div>
      </section>
    </div>
  );
};

export default Main;
