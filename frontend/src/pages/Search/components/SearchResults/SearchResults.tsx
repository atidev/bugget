import { useUnit } from "effector-react";
import { $searchResult, $usersStore } from "@/store/search";
import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import getStatusMeta from "@/utils/getStatusMeta";
import { useNavigate } from "react-router-dom";
import { ReportResponse } from "@/api/searchReports/models";
import { buildFullAppUrl } from "@/utils/buildFullUrl";

const getLatestUpdateDate = (report: ReportResponse) => {
  let latestTime: number = new Date(report.updatedAt).getTime();
  report.bugs?.forEach((bug) => {
    const bugTime = new Date(bug.updatedAt).getTime();
    if (bugTime !== null && (latestTime === null || bugTime > latestTime)) {
      latestTime = bugTime;
    }
    bug.comments.forEach((comment) => {
      const commentTime = new Date(comment.updatedAt).getTime();
      if (
        commentTime !== null &&
        (latestTime === null || commentTime > latestTime)
      ) {
        latestTime = commentTime;
      }
    });
  });
  return latestTime;
};

const SearchResults = () => {
  const [searchResult, usersStore] = useUnit([$searchResult, $usersStore]);
  const navigate = useNavigate();

  const handleClick = (
    e: React.MouseEvent<HTMLDivElement>,
    reportId: number
  ) => {
    const reportPath = `reports/${reportId}`;
    const fullUrl = buildFullAppUrl(reportPath);

    if (e.ctrlKey || e.metaKey || e.button === 1) {
      window.open(fullUrl, "_blank");
    } else {
      navigate(fullUrl);
    }
  };

  const handleMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.button === 1) {
      e.preventDefault();
    }
  };

  return (
    <div className="space-y-4">
      {searchResult?.reports?.map((report) => {
        const statusMeta = getStatusMeta("report", report.status);
        const latestUpdateDate = getLatestUpdateDate(report);

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
              Автор: {usersStore[report.creatorUserId]?.name ?? "—"} • Создан:
              {formatDistanceToNow(new Date(report.createdAt), {
                addSuffix: true,
                locale: ru,
              })}
            </div>
            <div className="text-sm text-base-content/70">
              Последнее обновление:{" "}
              {formatDistanceToNow(latestUpdateDate, {
                addSuffix: true,
                locale: ru,
              })}
            </div>
            <div className="text-sm text-base-content/70">
              Ответственный: {usersStore[report.responsibleUserId]?.name ?? "—"}
            </div>
            {!!report.bugs?.length && (
              <div className="text-sm mt-1">
                🐞 Багов: {report.bugs.length} • 💬 Комментариев:{" "}
                {report.bugs.reduce((sum, bug) => sum + bug.comments.length, 0)}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
};

export default SearchResults;
