import { useNavigate } from "react-router-dom";

import "./ReportSummary.css";
import { Report } from "@/types/report";

type ReportProps = {
  report: Report;
  highlight?: boolean;
};

const ReportSummary = ({ report, highlight }: ReportProps) => {
  const navigate = useNavigate();

  return (
    <div
      key={report.id}
      className={`card card-border shadow-lg cursor-pointer hover:bg-base-200 ${
        highlight ? "border-error" : "border-gray-300"
      }`}
      onClick={() => navigate(`/reports/${report.id}`)}
    >
      <div className="card-body p-4">
        <h3 className="card-title text-lg font-semibold">{report.title}</h3>
        <p className="text-sm dark:text-stone-300">
          Ответственный: {report.responsible?.name}
        </p>
        <p className="text-sm dark:text-stone-200">
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
