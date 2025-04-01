import { createEffect, createEvent, createStore, sample } from "effector";
import { createBugFx } from "./newBug";
import { $initialReportForm, clearReport } from "./report";
import { updateBugApi } from "@/api/reports/bug";
import { BugUpdateRequest } from "@/api/reports/models";
import { Bug } from "@/types/bug";

type UpdateBugParams = {
  reportId: number;
  bugId: number;
  bug: BugUpdateRequest;
};

export const updateBugFx = createEffect(
  async ({ reportId, bugId, bug }: UpdateBugParams) => {
    return await updateBugApi(reportId, bugId, bug);
  }
);

export const updateBugEvent = createEvent<Partial<Bug>>();
export const updateBugApiEvent = createEvent<Partial<Bug>>();
export const resetBug = createEvent<number>();

// todo maybe replace with new Map()
export const $initialBugsByBugId = createStore<Record<number, Bug>>({})
  .on($initialReportForm, (_, report) =>
    report.bugs.reduce((acc: Record<number, Bug>, bug: Bug) => {
      if (bug.id) {
        acc[bug.id] = bug;
      }
      return acc;
    }, {})
  )
  .on(updateBugFx.done, (state, { result }) => {
    if (!result) return state;
    const { id, receive, expect, status } = result;
    const bug = state[id];

    if (!bug) return state;

    return {
      ...state,
      [id]: {
        ...bug,
        receive: receive ?? bug.receive,
        expect: expect ?? bug.expect,
        status: status ?? bug.status,
        isChanged: false,
      },
    };
  })
  .on(createBugFx.doneData, (state, newBug) => ({
    ...state,
    [newBug.id]: newBug,
  }))
  .reset(clearReport);

export const $bugsByBugId = createStore<Record<number, Bug>>({})
  .on($initialBugsByBugId, (_, bugs) => bugs)
  .on(updateBugEvent, (state, payload) => {
    const { id, receive, expect, status } = payload;
    if (!id || !state[id]) return state;
    const bug = state[id];

    const updatedBug = {
      ...bug,
      receive: receive ?? bug.receive,
      expect: expect ?? bug.expect,
      status: status ?? bug.status,
    };

    const initialBug = $initialBugsByBugId.getState()[id];
    const isChanged =
      initialBug.receive !== updatedBug.receive ||
      initialBug.expect !== updatedBug.expect ||
      initialBug.status !== updatedBug.status;

    return {
      ...state,
      [id]: { ...updatedBug, isChanged },
    };
  })
  .on(resetBug, (state, bugId) => {
    const initialBug = $initialBugsByBugId.getState()[bugId];

    if (!initialBug) return state;

    return {
      ...state,
      [bugId]: { ...initialBug, isChanged: false },
    };
  })
  .reset(clearReport);

export const $bugsIds = createStore<number[]>([])
  .on($initialBugsByBugId, (_, bugs) => Object.keys(bugs).map(Number))
  .reset(clearReport);

sample({
  source: updateBugApiEvent,
  fn: (bug) =>
    ({
      reportId: bug.reportId,
      bugId: bug.id,
      bug: {
        expect: bug.expect ?? null,
        receive: bug.receive ?? null,
        status: bug.status ?? null,
      },
    } as UpdateBugParams),
  target: updateBugFx,
});
