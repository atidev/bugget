import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";

import { createBug, updateBug } from "@/api/bugs";
import {
  CreateBugRequest,
  CreateBugResponse,
  PatchBugRequest,
  PatchBugResponse,
} from "@/api/bugs/models";
import { BugStatuses } from "@/const";
import { BugClientEntity, BugFormData, BugUpdateData } from "@/types/bug";
import {
  CreateBugSocketResponse,
  PatchBugSocketResponse,
} from "@/webSocketApi/models";

/**
 * Эффекты для API
 */
export const createBugFx = createEffect<
  { reportId: number; data: CreateBugRequest },
  CreateBugResponse & { reportId: number }
>(async ({ reportId, data }) => {
  try {
    const result = await createBug(reportId, data);
    return { ...result, reportId };
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
 * События
 */

// внутренние события
export const setBugsEvent =
  createEvent<{ reportId: number; bugs: BugClientEntity[] }>();
export const createBugEvent =
  createEvent<{ reportId: number; data: BugFormData }>();
export const clearBugsEvent = createEvent<void>();

// пользовательские действия
export const updateBugDataEvent = createEvent<BugUpdateData>();
export const changeBugStatusEvent =
  createEvent<{ bugId: number; status: BugStatuses }>();

// события сокетов
export const createBugSocketEvent = createEvent<CreateBugSocketResponse>();
export const patchBugSocketEvent =
  createEvent<{ bugId: number; patch: PatchBugSocketResponse }>();

/**
 * Основные сторы
 */

// Стор для всех багов по id репорта
export const $bugsStore = createStore<Record<number, BugClientEntity>>({})
  .on(setBugsEvent, (state, { bugs }) => {
    const bugsById = bugs.reduce((acc, bug) => {
      acc[bug.id] = bug;
      return acc;
    }, {} as Record<number, BugClientEntity>);
    return { ...state, ...bugsById };
  })
  .on(createBugFx.doneData, (state, newBug) => ({
    ...state,
    [newBug.id]: {
      ...newBug,
      reportId: newBug.reportId,
      attachments: null,
      comments: null,
    } as BugClientEntity,
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
  .on(patchBugSocketEvent, (state, { bugId, patch }) => {
    const existingBug = state[bugId];
    if (!existingBug) return state;

    return {
      ...state,
      [bugId]: {
        ...existingBug,
        receive: patch.receive || existingBug.receive,
        expect: patch.expect || existingBug.expect,
        status: patch.status || existingBug.status,
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
  .on(createBugFx.doneData, (state, newBug) => {
    const bugIds = state[newBug.reportId] || [];
    return {
      ...state,
      [newBug.reportId]: [...bugIds, newBug.id],
    };
  })
  .reset(clearBugsEvent);

/**
 * Combined-сторы
 */

// Combined store из всех багов и id багов для каждого репорта
export const $bugsData = combine(
  $bugsStore,
  $reportBugsStore,
  (bugs, reportBugs) => ({ bugs, reportBugs })
);

/** Сэмплы */

// Создание бага
sample({
  clock: createBugEvent,
  fn: ({ reportId, data }) => ({
    reportId,
    data,
  }),
  target: createBugFx,
});

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
