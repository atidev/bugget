import { useNavigate } from "react-router-dom";
import { useUnit } from "effector-react";
import { ReportResponse } from "@/api/reports/models";
import { $usersStore } from "@/store/reportsDashboard";

type ReportProps = {
  report: ReportResponse;
  highlight?: boolean;
};

const ReportSummary = ({ report, highlight }: ReportProps) => {
  const navigate = useNavigate();
  const users = useUnit($usersStore);

  const responsibleUserName =
    users[report.responsibleUserId]?.name || report.responsibleUserId;

  const participantsNames = (report.participantsUserIds || [])
    .map((id) => users[id]?.name)
    .filter(Boolean);

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
          Ответственный: {responsibleUserName}
        </p>
        {!!participantsNames.length && (
          <p className="text-sm dark:text-stone-200">
            Участники: {participantsNames.join(", ")}
          </p>
        )}
      </div>
    </div>
  );
};

export default ReportSummary;
