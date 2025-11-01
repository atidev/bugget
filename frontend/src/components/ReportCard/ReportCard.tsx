import { BugStatuses, reportStatusMap } from "@/const";
import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { buildFullAppUrl } from "@/utils/buildFullUrl";

// Универсальный тип для поддержки разных версий ReportResponse
type ReportData = {
  id: number;
  title: string;
  status: number;
  createdAt: string;
  responsibleUserId: string;
  participantsUserIds?: string[] | null;
  bugs?: Array<{
    status: number;
    comments?: Array<{ id: number }> | null;
  }> | null;
};

type ReportCardProps = {
  report: ReportData;
  usersStore?: Record<string, { name: string }>;
  className?: string;
};

const maxParticipantsDisplayCount = 3;

const ReportCard = ({
  report,
  usersStore = {},
  className = "",
}: ReportCardProps) => {
  const navigate = useNavigate();
  const statusMeta = reportStatusMap[Number(report.status)];

  // Обработчик клика с поддержкой открытия в новой вкладке
  const handleClick = (e: React.MouseEvent<HTMLDivElement>) => {
    const reportPath = `reports/${report.id}`;
    const fullUrl = buildFullAppUrl(reportPath);

    if (e.ctrlKey || e.metaKey || e.button === 1) {
      window.open(fullUrl, "_blank");
      e.preventDefault();
    } else {
      navigate(fullUrl);
    }
  };

  const handleMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.button === 1) {
      e.preventDefault();
    }
  };

  // Подсчитываем открытые и всего багов
  const bugStats = useMemo(() => {
    if (!report.bugs || report.bugs.length === 0) {
      return null;
    }

    const resolvedBugs = report.bugs.filter(
      (bug) => bug.status !== BugStatuses.ACTIVE
    ).length;
    const totalBugs = report.bugs.length;

    return { resolved: resolvedBugs, total: totalBugs };
  }, [report.bugs]);

  // Подсчитываем количество комментариев
  const commentsCount = useMemo(() => {
    if (!report.bugs || report.bugs.length === 0) {
      return 0;
    }

    return report.bugs.reduce(
      (sum, bug) => sum + (bug.comments?.length || 0),
      0
    );
  }, [report.bugs]);

  // Получаем имя ответственного
  const responsibleUserName = usersStore[report.responsibleUserId]?.name;

  // Получаем участников (макс 3 для отображения), исключая ответственного
  const participants = useMemo(() => {
    if (
      !report.participantsUserIds ||
      report.participantsUserIds.length === 0
    ) {
      return [];
    }

    return report.participantsUserIds
      .filter((p) => p !== report.responsibleUserId)
      .slice(0, maxParticipantsDisplayCount)
      .map((userId) => ({
        id: userId,
        name: usersStore[userId]?.name || userId,
      }));
  }, [report.participantsUserIds, usersStore]);

  const totalParticipants =
    report.participantsUserIds?.filter((id) => id !== report.responsibleUserId)
      .length || 0;

  // Форматируем дату в формате "4 апр"
  const formattedDate = useMemo(() => {
    const date = new Date(report.createdAt);
    return date.toLocaleDateString("ru-RU", {
      day: "numeric",
      month: "short",
    });
  }, [report.createdAt]);

  return (
    <div
      className={`card bg-base-100 cursor-pointer border border-base-300 hover:bg-base-200 ${className}`}
      onClick={handleClick}
      onMouseDown={handleMouseDown}
    >
      <div className="card-body p-3 gap-2">
        {/* Верхняя строка: номер, статус, заголовок, дата */}
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-start gap-2 flex-1 min-w-0">
            {/* Номер отчета и статус */}
            <div className="flex items-center gap-2 flex-shrink-0">
              <span className="text-sm font-semibold text-base-content/70">
                #{report.id}
              </span>

              {/* Индикатор статуса - круглая иконка */}
              <statusMeta.icon className={`w-4 h-4 ${statusMeta.iconColor}`} />
            </div>

            {/* Заголовок */}
            <h3
              className="text-sm font-bold line-clamp-1 flex-1 min-w-0"
              title={report.title}
            >
              {report.title}
            </h3>
          </div>

          {/* Дата */}
          <span className="text-sm text-base-content/50 flex-shrink-0">
            {formattedDate}
          </span>
        </div>

        {/* Нижняя строка: метаданные */}
        <div className="flex items-center justify-between gap-3">
          {/* Ответственный и участники */}
          <div className="flex items-center gap-2">
            {/* Ответственный */}
            {responsibleUserName && (
              <div className="flex items-center gap-2">
                <div
                  className="w-6 h-6 rounded-full bg-accent text-accent-content flex items-center justify-center text-xs border-2 border-base-100 hover:z-10 transition-transform hover:scale-110 cursor-pointer"
                  title={`Ответственный: ${responsibleUserName}`}
                  aria-label={responsibleUserName}
                >
                  {responsibleUserName?.charAt(0).toUpperCase()}
                </div>
                <span className="text-xs text-base-content">
                  {responsibleUserName}
                </span>
              </div>
            )}

            {/* Участники */}
            {participants.length > 0 && (
              <div className="flex items-center ml-2">
                <div className="flex -space-x-2 overflow-visible">
                  {participants.map((p) => (
                    <div
                      key={p.id}
                      className="tooltip tooltip-bottom"
                      data-tip={p.name}
                    >
                      <button
                        type="button"
                        className="w-6 h-6 rounded-full bg-primary text-primary-content flex items-center justify-center text-[10px] font-medium border-2 border-base-100
                       transition-transform hover:scale-110 hover:z-20 focus:scale-110 focus:z-20 outline-none"
                        aria-label={`Участник: ${p.name}`}
                        tabIndex={0}
                      >
                        {p.name?.charAt(0).toUpperCase()}
                      </button>
                    </div>
                  ))}
                </div>

                {totalParticipants > maxParticipantsDisplayCount && (
                  <div
                    className="tooltip tooltip-bottom ml-0.5"
                    data-tip={(report.participantsUserIds ?? [])
                      .slice(maxParticipantsDisplayCount)
                      .filter((id) => id !== report.responsibleUserId)
                      .map((id) => usersStore[id]?.name || id)
                      .join(", ")}
                  >
                    <span className="text-xs text-base-content/60 cursor-default">
                      +{totalParticipants - maxParticipantsDisplayCount}
                    </span>
                  </div>
                )}
              </div>
            )}
          </div>

          {/* Правый блок: баги, комментарии */}
          <div className="flex items-center gap-2">
            {/* Баги */}
            {bugStats && (
              <div className="flex items-center gap-1.5">
                <span className="text-base">🐞</span>
                <span
                  className={`text-sm font-semibold
                    }`}
                >
                  {bugStats.resolved}/{bugStats.total}
                </span>
              </div>
            )}

            {/* Комментарии */}
            {commentsCount > 0 && (
              <div className="flex items-center gap-1.5">
                <span className="text-base">💬</span>
                <span className="text-sm font-semibold">{commentsCount}</span>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ReportCard;
