import { useUnit } from "effector-react";
import { $searchResult } from "@/store/search";
import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import { getStatusMeta } from "@/constants/status";
import { useNavigate } from "react-router-dom";
import { Report } from "@/types/report";

const getLatestUpdateDate = (report: Report) => {
  let latestTime = new Date(report.updatedAt).getTime();
  report.bugs.forEach((bug) => {
    const bugTime = new Date(bug.updatedAt).getTime();
    if (bugTime > latestTime) {
      latestTime = bugTime;
    }
    bug.comments.forEach((comment) => {
      const commentTime = new Date(comment.updatedAt).getTime();
      if (commentTime > latestTime) {
        latestTime = commentTime;
      }
    });
  });
  return new Date(latestTime);
};

const SearchResults = () => {
  const [searсhResult] = useUnit([$searchResult]);
  const navigate = useNavigate();

  const handleClick = (
    e: React.MouseEvent<HTMLDivElement>,
    reportId: number
  ) => {
    if (e.ctrlKey || e.metaKey || e.button === 1) {
      window.open(`/reports/${reportId}`, "_blank");
    } else {
      navigate(`/reports/${reportId}`);
    }
  };

  const handleMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.button === 1) {
      e.preventDefault();
    }
  };

  return (
    <div className="space-y-4">
      {searсhResult?.reports?.map((report) => {
        const statusMeta = getStatusMeta("report", report.status);

        return (
          <div
            key={report.id}
            className={`card card-border ${statusMeta.border} p-4 hover:bg-base-200 cursor-pointer`}
            onClick={(e) => handleClick(e, report.id)}
            onMouseDown={handleMouseDown}
          >
            <div className="flex justify-between items-start">
              <div className="font-bold text-base">
                #{report.id} {report.title}
              </div>
              <span className={`badge ${statusMeta.color}`}>
                {statusMeta.title}
              </span>
            </div>

            <div className="text-sm text-base-content/70 mt-1">
              Автор: {report.creator.name} • Создан:{" "}
              {formatDistanceToNow(new Date(report.createdAt), {
                addSuffix: true,
                locale: ru,
              })}
            </div>
            <div className="text-sm text-base-content/70">
              Последнее обновление:{" "}
              {formatDistanceToNow(getLatestUpdateDate(report), {
                addSuffix: true,
                locale: ru,
              })}
            </div>
            <div className="text-sm text-base-content/70">
              Ответственный: {report.responsible?.name ?? "—"}
            </div>
            <div className="text-sm mt-1">
              🐞 Багов: {report.bugs.length} • 💬 Комментариев:{" "}
              {report.bugs.reduce((sum, bug) => sum + bug.comments.length, 0)}
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default SearchResults;
