import { ListReportsResponse } from "@/api/reports/models";
import ReportCard from "@/components/ReportCard";
import { useUnit } from "effector-react";
import { $usersStore } from "@/store/dashboard";
import { useState } from "react";
import { ChevronDown, ChevronRight } from "lucide-react";
import { lastReportsDashboardTake } from "@/const";
import { Link } from "react-router-dom";
import { buildFullAppUrl } from "@/utils/buildFullUrl";

type Props = {
  data: ListReportsResponse;
  className?: string;
  onExpand?: () => void;
};

const LastReportsSection = ({ data, className, onExpand }: Props) => {
  const usersStore = useUnit($usersStore);
  const [isExpanded, setIsExpanded] = useState(false);

  const searchPath = "search";
  const fullUrl = buildFullAppUrl(searchPath);

  const handleToggle = () => {
    if (!isExpanded && onExpand) {
      onExpand();
    }
    setIsExpanded(!isExpanded);
  };

  return (
    <section className="flex flex-col gap-2">
      <div
        className="text-sm text-base-content/70 flex items-center gap-1 cursor-pointer select-none"
        onClick={handleToggle}
      >
        Недавно решённые
        {isExpanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
      </div>
      {isExpanded && !!data.reports.length && (
        <>
          <div className="flex flex-col gap-1">
            {data.reports.map((report) => (
              <ReportCard
                key={report.id}
                report={report}
                usersStore={usersStore}
                className={className}
              />
            ))}
          </div>
          {data.total > lastReportsDashboardTake && (
            <div>
              <span className="text-xs text-base-content/50">
                Больше репортов{" "}
                <Link to={fullUrl} className="underline">
                  в поиске
                </Link>{" "}
              </span>
            </div>
          )}
        </>
      )}
    </section>
  );
};

export default LastReportsSection;
