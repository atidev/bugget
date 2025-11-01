import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { fetchReportsList } from "@/api/reports";
import { ListReportsResponse } from "@/api/reports/models";
import { fetchUsers } from "@/api/users";
import { UserResponse } from "@/types/user";
import { ReportStatuses, lastReportsDashboardTake } from "@/const";

// Событие открытия страницы дашборда
export const dashboardPageOpened = createEvent();

export const loadReportsFx = createEffect(async (userId: string) => {
  const data = await fetchReportsList(userId, null, [
    Number(ReportStatuses.BACKLOG),
    Number(ReportStatuses.IN_PROGRESS),
  ]);
  return data;
});

export const fetchUsersFx = createEffect<string[], UserResponse[]>(
  async (userIds) => {
    if (userIds.length === 0) return [];
    return await fetchUsers(userIds);
  }
);

export const $dashboardReports = createStore<ListReportsResponse>({
  total: 0,
  reports: [],
}).on(loadReportsFx.doneData, (_, reports) => reports);

// стор для хранения пользователей по ID
export const $usersStore = createStore<Record<string, UserResponse>>({}).on(
  fetchUsersFx.doneData,
  (state, users) => {
    const usersById = users.reduce(
      (acc, user) => {
        acc[user.id] = user;
        return acc;
      },
      {} as Record<string, UserResponse>
    );
    return { ...state, ...usersById };
  }
);

// получаем все уникальные id пользователей для загрузки
export const $allUserIdsStore = combine($dashboardReports, (data) => {
  const allIds = new Set<string>();
  data.reports.forEach((report) => {
    allIds.add(report.responsibleUserId);
    allIds.add(report.creatorUserId);
    report.participantsUserIds?.forEach((id) => allIds.add(id));
  });
  return Array.from(allIds).filter(Boolean);
});

// загрузка пользователей после загрузки отчетов
sample({
  clock: loadReportsFx.doneData,
  source: $allUserIdsStore,
  filter: (userIds) => userIds.length > 0,
  target: fetchUsersFx,
});

// Событие открытия секции недавно решённых
export const recentlyResolvedSectionOpened = createEvent();

// Эффект для загрузки недавно решённых репортов (RESOLVED или REJECTED)
export const loadRecentlyResolvedFx = createEffect(async (userId: string) => {
  const data = await fetchReportsList(
    userId,
    null,
    [Number(ReportStatuses.RESOLVED), Number(ReportStatuses.REJECTED)],
    0,
    lastReportsDashboardTake
  );
  return data;
});

// Стор для недавно решённых репортов
export const $recentlyResolvedReports = createStore<ListReportsResponse>({
  total: 0,
  reports: [],
}).on(loadRecentlyResolvedFx.doneData, (_, data) => data);

// загрузка пользователей после загрузки недавно решённых репортов
sample({
  clock: loadRecentlyResolvedFx.doneData,
  source: $recentlyResolvedReports,
  fn: (data) => {
    const allIds = new Set<string>();
    data.reports.forEach((report) => {
      allIds.add(report.responsibleUserId);
      allIds.add(report.creatorUserId);
      report.participantsUserIds?.forEach((id) => allIds.add(id));
    });
    return Array.from(allIds).filter(Boolean);
  },
  filter: (data) => data.reports.length > 0,
  target: fetchUsersFx,
});
