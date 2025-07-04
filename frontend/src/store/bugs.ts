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
import { BugEntity, BugFormData, BugUpdateData } from "@/types/bug";
import {
  CreateBugSocketResponse,
  PatchBugSocketResponse,
} from "@/webSocketApi/models";

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
 * События
 */

// пользовательские действия
export const setBugsEvent =
  createEvent<{ reportId: number; bugs: BugEntity[] }>();
export const addBugEvent =
  createEvent<{ reportId: number; data: BugFormData }>();
export const updateBugDataEvent = createEvent<BugUpdateData>();
export const changeBugStatusEvent =
  createEvent<{ bugId: number; status: BugStatuses }>();
export const clearBugsEvent = createEvent<void>();

// события сокетов
export const createBugSocketEvent = createEvent<CreateBugSocketResponse>();
export const patchBugSocketEvent =
  createEvent<{ bugId: number; patch: PatchBugSocketResponse }>();

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
  .reset(clearBugsEvent);

/**
 * Computed сторы
 */

// Combined store из всех багов и id багов для каждого репорта
export const $bugsData = combine(
  $bugsStore,
  $reportBugsStore,
  (bugs, reportBugs) => ({ bugs, reportBugs })
);

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
