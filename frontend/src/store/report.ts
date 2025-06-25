import { createReport, fetchReport, patchReport } from "@/api/reports";
import {
  PatchReportRequest,
  PatchReportResponse,
  ReportResponse,
  CreateReportResponse,
} from "@/api/reports/models";

import { ReportStatuses } from "@/const";
import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { PatchReportSocketResponse } from "@/webSocketApi/models";
import { $searchResult } from "@/storeObsolete/search";

//// эффекты
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

//// события
export const changeTitleEvent = createEvent<string>();
export const saveTitleEvent = createEvent<void>();
export const changeStatusEvent = createEvent<ReportStatuses>();
export const changeResponsibleUserIdEvent = createEvent<string>();
export const patchReportSocketEvent = createEvent<PatchReportSocketResponse>();
export const updateResponsibleUserIdEvent = createEvent<string>();
export const updateCreatorUserIdEvent = createEvent<string>();
export const updateReportPathIdEvent = createEvent<number | null>();

//// сторы
export const $reportPathStore = createStore<number | null>(null).on(
  updateReportPathIdEvent,
  (_, reportPath) => reportPath
);

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
  }));

export const $titleStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.title)
  .on(patchReportSocketEvent, (state, report) => report.title ?? state)
  .on(changeTitleEvent, (_, title) => title);

export const $statusStore = createStore<ReportStatuses>(ReportStatuses.BACKLOG)
  .on(getReportFx.doneData, (_, report) => report.status)
  .on(patchReportSocketEvent, (state, report) => report.status ?? state);

export const $responsibleUserIdStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.responsibleUserId)
  .on(
    patchReportSocketEvent,
    (state, report) => report.responsibleUserId ?? state
  );

export const $creatorUserIdStore = createStore<string>("").on(
  getReportFx.doneData,
  (_, report) => report.creatorUserId
);

export const $pastResponsibleUserIdStore = createStore<string>("")
  .on(getReportFx.doneData, (_, report) => report.pastResponsibleUserId)
  .on(
    patchReportFx.doneData,
    (state, report) => report.pastResponsibleUserId ?? state
  )
  .on(
    patchReportSocketEvent,
    (state, report) => report.pastResponsibleUserId ?? state
  );

export const $updatedAtStore = createStore<string>(new Date().toISOString())
  .on(getReportFx.doneData, (_, report) => report.updatedAt)
  .on(createReportFx.doneData, (_, report) => report.updatedAt)
  .on(patchReportFx.doneData, (_, report) => report.updatedAt)
  .on(patchReportSocketEvent, (_, report) => {
    console.log("🔄 [Report] Updated at:", report.updatedAt);
    return report.updatedAt;
  });

export const $reportIdStore = createStore<number | null>(null).on(
  $initialReportStore,
  (_, report) => report?.id ?? null
);

export const $participantsUserIds = createStore<string[]>([]).on(
  getReportFx.doneData,
  (_, report) => report.participantsUserIds
);

// получаем имя ответственного из результатов поиска отчетов
export const $responsibleUserNameStore = combine(
  $reportIdStore,
  $searchResult,
  (reportId, searchResult) => {
    if (!reportId || !searchResult.reports) return "";

    const currentReport = searchResult.reports.find(
      (report) => report.id === reportId
    );
    return currentReport?.responsible?.name || "";
  }
);

// получаем участников из результатов поиска отчетов
export const $participantsWithNamesStore = combine(
  $reportIdStore,
  $searchResult,
  (reportId, searchResult) => {
    if (!reportId || !searchResult.reports) return [];

    const currentReport = searchResult.reports.find(
      (report) => report.id === reportId
    );
    return currentReport?.participants || [];
  }
);

//// связи
// загрузка открытого репорта
sample({
  clock: updateReportPathIdEvent,
  filter: (pathId) => pathId !== null,
  target: getReportFx,
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
