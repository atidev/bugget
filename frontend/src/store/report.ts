import { createReport, getReport, patchReport } from "@/api/reports";
import { PatchReportRequest, PatchReportResponse, ReportResponse, ReportSummaryResponse } from "@/api/reports/models";
import { ReportStatuses } from "@/const";
import { createEffect, createEvent, createStore, sample, attach } from "effector";
import { ReportPatchSocketResponse } from "@/webSocketApi/models";
import { $userData } from "./user";

export const getReportFx = createEffect<number, ReportResponse>(async (id) => {
    return await getReport(id);
});

export const createReportFx = createEffect<string, ReportSummaryResponse>(async (title) => {
    return await createReport({ title });
});

export const patchReportFx = createEffect<{ id: number, patchRequest: PatchReportRequest }, PatchReportResponse>(
    async ({ id, patchRequest }) => {
        return await patchReport(id, patchRequest);
    });

export const changeTitleEvent = createEvent<string>();
export const saveTitleEvent = createEvent<void>();
export const changeStatusEvent = createEvent<ReportStatuses>();
export const changeResponsibleUserIdEvent = createEvent<string>();
export const patchReportSocketEvent = createEvent<ReportPatchSocketResponse>();

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
        bugs: []
    }));

export const $titleStore = createStore<string>("")
    .on(getReportFx.doneData, (_, report) => report.title)
    .on(createReportFx.doneData, (_, report) => report.title)
    .on(patchReportFx.doneData, (_, report) => report.title)
    .on(patchReportSocketEvent, (state, report) => report.title ?? state)
    .on(changeTitleEvent, (_, title) => title);

export const $statusStore = createStore<ReportStatuses>(ReportStatuses.BACKLOG)
    .on(getReportFx.doneData, (_, report) => report.status)
    .on(createReportFx.doneData, (_, report) => report.status)
    .on(patchReportFx.doneData, (_, report) => report.status)
    .on(patchReportSocketEvent, (state, report) => report.status ?? state)
    .on(changeStatusEvent, (_, status) => status);

// События для обновления ID пользователя
export const updateResponsibleUserIdEvent = createEvent<string>();
export const updateCreatorUserIdEvent = createEvent<string>();

// Эффект для инициализации ID пользователя
const initUserIdsFx = createEffect(() => {
    const userData = $userData.getState();
    if (userData.isUserLoaded) {
        updateResponsibleUserIdEvent(userData.userId);
        updateCreatorUserIdEvent(userData.userId);
    }
});

// Запускаем инициализацию при монтировании компонента
initUserIdsFx();

export const $responsibleUserIdStore = createStore<string>("")
    .on(updateResponsibleUserIdEvent, (state, userId) => state || userId)
    .on(getReportFx.doneData, (_, report) => report.responsibleUserId)
    .on(createReportFx.doneData, (_, report) => report.responsibleUserId)
    .on(patchReportFx.doneData, (_, report) => report.responsibleUserId)
    .on(patchReportSocketEvent, (state, report) => report.responsibleUserId ?? state);

export const $creatorUserIdStore = createStore<string>("")
    .on(updateCreatorUserIdEvent, (state, userId) => state || userId)
    .on(getReportFx.doneData, (_, report) => report.creatorUserId)
    .on(createReportFx.doneData, (_, report) => report.creatorUserId);

export const $pastResponsibleUserIdStore = createStore<string>("")
    .on($responsibleUserIdStore, (_, responsibleUserId) => responsibleUserId)
    .on(getReportFx.doneData, (_, report) => report.pastResponsibleUserId)
    .on(createReportFx.doneData, (_, report) => report.pastResponsibleUserId)
    .on(patchReportFx.doneData, (_, report) => report.pastResponsibleUserId)
    .on(patchReportSocketEvent, (state, report) => report.pastResponsibleUserId ?? state);

export const $updatedAtStore = createStore<string>(new Date().toISOString())
    .on(getReportFx.doneData, (_, report) => report.updatedAt)
    .on(createReportFx.doneData, (_, report) => report.updatedAt)
    .on(patchReportFx.doneData, (_, report) => report.updatedAt)
    .on(patchReportSocketEvent, (state, report) => report.updatedAt ?? state);

export const $reportIdStore = createStore<number | null>(null)
    .on($initialReportStore, (_, report) => report?.id ?? null);

sample({
    clock: saveTitleEvent,
    source: {
        id: $reportIdStore,
        title: $titleStore
    },
    filter: ({ id }) => id === null,
    fn: ({ title }) => title,
    target: createReportFx
});

sample({
    clock: saveTitleEvent,
    source: {
        id: $reportIdStore,
        title: $titleStore
    },
    filter: ({ id }) => id !== null,
    fn: ({ id, title }) => ({
        id: id!,
        patchRequest: { title }
    }),
    target: patchReportFx
});

sample({
    clock: changeResponsibleUserIdEvent,
    source: {
        id: $reportIdStore,
        responsibleUserId: $responsibleUserIdStore
    },
    filter: ({ id }) => id !== null,
    fn: ({ id, responsibleUserId }) => {
        if (!id) throw new Error("Initial report not found");
        patchReportFx({
            id,
            patchRequest: { responsibleUserId }
        });
    }
});

sample({
    clock: changeStatusEvent,
    source: {
        id: $reportIdStore,
        status: $statusStore
    },
    filter: ({ id }) => id !== null,
    fn: ({ id, status }) => {
        if (!id) throw new Error("Initial report not found");
        patchReportFx({
            id,
            patchRequest: { status }
        });
    }
});