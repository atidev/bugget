import { createBug, updateBug } from "@/api/bugs";
import {
  CreateBugRequest,
  CreateBugResponse,
  PatchBugRequest,
  PatchBugResponse,
} from "@/api/bugs/models";
import { BugStatuses } from "@/const";
import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { $reportIdStore, getReportFx } from "./report";
import { BugEntity, BugFormData, BugUpdateData } from "@/types/bug";

/**
 * Эффекты для API
 */
export const createBugFx = createEffect<
  { reportId: number; data: CreateBugRequest },
  CreateBugResponse
>(async ({ reportId, data }) => {
  try {
    const result = await createBug(reportId, data);
    return result;
  } catch (error) {
    console.error("❌ createBugFx error:", error);
    throw error;
  }
});

export const updateBugFx = createEffect<
  { reportId: number; bugId: number; data: PatchBugRequest },
  PatchBugResponse
>(async ({ reportId, bugId, data }) => {
  try {
    const result = await updateBug(reportId, bugId, data);
    return result;
  } catch (error) {
    console.error("❌ updateBugFx error:", error);
    throw error;
  }
});

/**
 * События для пользовательских действий
 */
export const setBugsEvent =
  createEvent<{ reportId: number; bugs: BugEntity[] }>();
export const addBugEvent =
  createEvent<{ reportId: number; data: BugFormData }>();
export const updateBugDataEvent = createEvent<BugUpdateData>();
export const changeBugStatusEvent =
  createEvent<{ bugId: number; status: BugStatuses }>();
export const clearBugsEvent = createEvent<void>();

/**
 * Основные сторы
 */

// Стор для всех багов по id репорта
export const $bugsStore = createStore<Record<number, BugEntity>>({})
  .on(setBugsEvent, (state, { bugs }) => {
    const bugsById = bugs.reduce((acc, bug) => {
      acc[bug.id] = bug;
      return acc;
    }, {} as Record<number, BugEntity>);
    return { ...state, ...bugsById };
  })
  .on(createBugFx.doneData, (state, newBug) => ({
    ...state,
    [newBug.id]: {
      ...newBug,
      reportId: newBug.id, // будет переопределено в setBugsEvent
      attachments: null,
      comments: null,
    } as BugEntity,
  }))
  .on(updateBugFx.doneData, (state, updatedBug) => {
    const existingBug = state[updatedBug.id];
    if (!existingBug) return state;

    return {
      ...state,
      [updatedBug.id]: {
        ...existingBug,
        receive: updatedBug.receive,
        expect: updatedBug.expect,
        status: updatedBug.status,
        updatedAt: updatedBug.updatedAt,
      },
    };
  })
  .reset(clearBugsEvent);

// Список id багов для каждого репорта
export const $reportBugsStore = createStore<Record<number, number[]>>({})
  .on(setBugsEvent, (state, { reportId, bugs }) => ({
    ...state,
    [reportId]: bugs.map((bug) => bug.id),
  }))
  .reset(clearBugsEvent);

/**
 * Computed сторы
 */

// Получение багов для конкретного репорта
export const getBugsForReport = (reportId: number | null) =>
  combine($bugsStore, $reportBugsStore, (bugs, reportBugs) => {
    if (!reportId) return [];
    const bugIds = reportBugs[reportId] || [];
    return bugIds
      .map((id) => bugs[id])
      .filter(Boolean)
      .sort(
        (a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      );
  });

/**
 * Связи между сторами (Sample)
 */
// Обновление бага
sample({
  clock: updateBugDataEvent,
  fn: ({ bugId, reportId, data }) => ({
    reportId,
    bugId,
    data,
  }),
  target: updateBugFx,
});

// Изменение статуса бага
sample({
  clock: changeBugStatusEvent,
  source: $reportIdStore,
  filter: (reportId) => reportId !== null,
  fn: (reportId, { bugId, status }) => ({
    bugId,
    reportId: reportId!,
    data: { status },
  }),
  target: updateBugDataEvent,
});

// Загрузка багов при загрузке репорта
sample({
  clock: getReportFx.doneData,
  fn: (report) => ({
    reportId: report.id,
    bugs: report.bugs || [],
  }),
  target: setBugsEvent,
});
