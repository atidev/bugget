import { useNavigate } from "react-router-dom";

import "./ReportSummary.css";
import { Report } from "@/types/report";

interface ReportProps {
  report: Report;
  highlight?: boolean;
}

const ReportSummary = ({ report, highlight }: ReportProps) => {
  const navigate = useNavigate();

  return (
    <div
      key={report.id}
      className={`card border bg-base-300 shadow-lg cursor-pointer ${
        highlight ? "border-error bg-warning/10" : "border-gray-300"
      }`}
      onClick={() => navigate(`/reports/${report.id}`)}
    >
      <div className="card-body p-4 bg-base-300">
        <h3 className="card-title text-lg font-semibold dark:text-stone-50">
          {report.title}
        </h3>
        <p className="text-sm text-gray-600 dark:text-stone-300">
          Ответственный: {report.responsible.name}
        </p>
        <p className="text-sm text-gray-500 dark:text-stone-200">
          Участники:{" "}
          {report.participants
            .map((participant) => participant.name)
            .join(", ")}
        </p>
      </div>
    </div>
  );
};

export default ReportSummary;
