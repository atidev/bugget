import { useUnit } from "effector-react";
import { $searchResult } from "@/store/search";
import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import { getStatusMeta } from "@/constants/status";
import { useNavigate } from "react-router-dom";
import { Report } from "@/types/report";

const getLatestUpdateDate = (report: Report) => {
  return new Date(
    Math.max(
      new Date(report.updatedAt).getTime(),
      ...report.bugs.map((bug) =>
        Math.max(
          new Date(bug.updatedAt).getTime(),
          ...bug.comments.map((c) => new Date(c.updatedAt).getTime())
        )
      )
    )
  );
};

const SearchResults = () => {
  const [searhResult] = useUnit([$searchResult]);
  const navigate = useNavigate();

  return (
    <div className="space-y-4">
      {searhResult?.reports?.map((report) => {
        const statusMeta = getStatusMeta("report", report.status);

        return (
          <div
            key={report.id}
            className={`border ${statusMeta.border} rounded-lg p-4 hover:bg-base-100 cursor-pointer`}
            onClick={(e) => {
              if (e.ctrlKey || e.metaKey || e.button === 1) {
                window.open(`/reports/${report.id}`, "_blank");
              } else {
                navigate(`/reports/${report.id}`);
              }
            }}
            onMouseDown={(e) => e.button === 1 && e.preventDefault()}
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
