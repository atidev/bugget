import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { listReports } from "@/api/reports";
import { ReportResponse } from "@/api/reports/models";
import { fetchUsers } from "@/api/users";
import { UserResponse } from "@/types/user";

// Событие открытия страницы дашборда
export const dashboardPageOpened = createEvent();

export const loadReportsFx = createEffect(async (userId: string) => {
  const data = await listReports(userId);
  return data.reports;
});

export const fetchUsersFx = createEffect<string[], UserResponse[]>(
  async (userIds) => {
    if (userIds.length === 0) return [];
    return await fetchUsers(userIds);
  }
);

export const $reports = createStore<ReportResponse[]>([]).on(
  loadReportsFx.doneData,
  (_, reports) => reports
);

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
export const $allUserIdsStore = combine($reports, (reports) => {
  const allIds = new Set<string>();
  reports.forEach((report) => {
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
