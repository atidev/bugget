import { createReport, fetchReport, patchReport } from "@/api/reports";
import {
  PatchReportRequest,
  PatchReportResponse,
  ReportResponse,
  CreateReportResponse,
} from "@/api/reports/models";
import { fetchUsers } from "@/api/users";
import { UserResponse } from "@/types/user";

import { ReportStatuses } from "@/const";
import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { PatchReportSocketResponse } from "@/webSocketApi/models";

/**
 * Эффекты
 */
export const getReportFx = createEffect<number, ReportResponse>(async (id) => {
  return await fetchReport(id);
});

export const createReportFx = createEffect<string, CreateReportResponse>(
  async (title) => {
    return await createReport({ title });
  }
);

export const patchReportFx = createEffect<
  { id: number; patchRequest: PatchReportRequest },
  PatchReportResponse
>(async ({ id, patchRequest }) => {
  return await patchReport(id, patchRequest);
});

export const fetchUsersFx = createEffect<string[], UserResponse[]>(
  async (userIds) => {
    if (userIds.length === 0) return [];
    return await fetchUsers(userIds);
  }
);

/**
 * События
 */
export const changeTitleEvent = createEvent<string>();
export const saveTitleEvent = createEvent<void>();
export const changeStatusEvent = createEvent<ReportStatuses>();
export const changeResponsibleUserIdEvent = createEvent<string>();
export const patchReportSocketEvent = createEvent<PatchReportSocketResponse>();
export const updateResponsibleUserIdEvent = createEvent<string>();
export const updateCreatorUserIdEvent = createEvent<string>();
export const updateReportPathIdEvent = createEvent<number | null>();
export const clearReport = createEvent<void>();

/**
 * Сторы
 */
export const $reportPathStore = createStore<number | null>(null)
  .on(updateReportPathIdEvent, (_, reportPath) => reportPath)
  .reset(clearReport);

// источник данных для других сторов
export const $initialReportStore = createStore<ReportResponse | null>(null)
  .on(getReportFx.doneData, (_, report) => report)
  .on(createReportFx.doneData, (state, report) => ({
    ...state,
    id: report.id,
    title: report.title,
    status: report.status,
    responsibleUserId: report.responsibleUserId,
    pastResponsibleUserId: report.responsibleUserId,
    creatorUserId: report.creatorUserId,
    createdAt: report.createdAt,
    updatedAt: report.updatedAt,
    participantsUserIds: [],
    bugs: [],
  }))
  .reset(clearReport);

export const $titleStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.title)
  .on(patchReportSocketEvent, (state, report) => report.title ?? state)
  .on(changeTitleEvent, (_, title) => title)
  .reset(clearReport);

export const $statusStore = createStore<ReportStatuses>(ReportStatuses.BACKLOG)
  .on(getReportFx.doneData, (_, report) => report.status)
  .on(patchReportSocketEvent, (state, report) => report.status ?? state)
  .reset(clearReport);

export const $responsibleUserIdStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.responsibleUserId)
  .on(
    patchReportSocketEvent,
    (state, report) => report.responsibleUserId ?? state
  )
  .reset(clearReport);

export const $creatorUserIdStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.creatorUserId)
  .reset(clearReport);

export const $pastResponsibleUserIdStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.pastResponsibleUserId)
  .on(
    patchReportFx.doneData,
    (state, report) => report.pastResponsibleUserId ?? state
  )
  .on(
    patchReportSocketEvent,
    (state, report) => report.pastResponsibleUserId ?? state
  )
  .reset(clearReport);

export const $updatedAtStore = createStore<string>(new Date().toISOString())
  .on(getReportFx.doneData, (_, report) => report.updatedAt)
  .on(createReportFx.doneData, (_, report) => report.updatedAt)
  .on(patchReportFx.doneData, (_, report) => report.updatedAt)
  .on(patchReportSocketEvent, (_, report) => {
    console.log("🔄 [Report] Updated at:", report.updatedAt);
    return report.updatedAt;
  })
  .reset(clearReport);

export const $reportIdStore = createStore<number | null>(null)
  .on($initialReportStore, (_, report) => report?.id ?? null)
  .reset(clearReport);

export const $participantsUserIdsStore = createStore<string[]>([])
  .on(getReportFx.doneData, (_, report) => report.participantsUserIds)
  .reset(clearReport);

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

// получаем имя ответственного пользователя
export const $responsibleUserNameStore = combine(
  $responsibleUserIdStore,
  $usersStore,
  (responsibleUserId, users) => {
    if (!responsibleUserId) return "";
    return users[responsibleUserId]?.name || "";
  }
);

// получаем имя создателя отчета
export const $creatorUserNameStore = combine(
  $creatorUserIdStore,
  $usersStore,
  (creatorUserId, users) => {
    if (!creatorUserId) return "";
    return users[creatorUserId]?.name || "";
  }
);

// получаем участников с именами
export const $participantsWithNamesStore = combine(
  $participantsUserIdsStore,
  $usersStore,
  (participantsIds, users) => {
    return participantsIds
      .map((id) => users[id])
      .filter(Boolean)
      .map((user) => ({
        id: user.id,
        name: user.name,
      }));
  }
);

// получаем все уникальные id пользователей для загрузки
export const $allUserIdsStore = combine(
  $responsibleUserIdStore,
  $creatorUserIdStore,
  $participantsUserIdsStore,
  (responsibleUserId, creatorUserId, participantsIds) => {
    const allIds = [
      responsibleUserId,
      creatorUserId,
      ...participantsIds,
    ].filter(Boolean);
    return [...new Set(allIds)];
  }
);

/**
 * Связи
 */
// загрузка открытого репорта
sample({
  clock: updateReportPathIdEvent,
  filter: (pathId) => pathId !== null,
  target: getReportFx,
});

// загрузка пользователей после загрузки репорта
sample({
  clock: getReportFx.doneData,
  source: $allUserIdsStore,
  filter: (userIds) => userIds.length > 0,
  target: fetchUsersFx,
});

// загрузка пользователей при изменении списка участников через сокет
sample({
  clock: patchReportSocketEvent,
  source: $allUserIdsStore,
  filter: (userIds) => userIds.length > 0,
  target: fetchUsersFx,
});

// создание репорта
sample({
  clock: saveTitleEvent,
  source: {
    id: $reportIdStore,
    title: $titleStore,
  },
  filter: ({ id }) => id === null,
  fn: ({ title }) => title,
  target: createReportFx,
});

// изменение названия репорта
sample({
  clock: saveTitleEvent,
  source: {
    id: $reportIdStore,
    title: $titleStore,
  },
  filter: ({ id }) =>
    id !== null &&
    $initialReportStore.getState()?.title !== $titleStore.getState(),
  fn: ({ id, title }) => ({
    id: id!,
    patchRequest: { title },
  }),
  target: patchReportFx,
});

sample({
  clock: changeStatusEvent,
  source: $reportIdStore,
  filter: (id): id is number => id !== null,
  fn: (id, status) => ({
    id: id!,
    patchRequest: { status },
  }),
  target: patchReportFx,
});

sample({
  clock: changeStatusEvent,
  target: $statusStore,
});

sample({
  clock: changeResponsibleUserIdEvent,
  source: $reportIdStore,
  filter: (id): id is number => id !== null,
  fn: (id, responsibleUserId) => ({
    id: id!,
    patchRequest: { responsibleUserId },
  }),
  target: patchReportFx,
});

sample({
  clock: changeResponsibleUserIdEvent,
  target: $responsibleUserIdStore,
});
