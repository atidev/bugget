import { createEffect, createEvent, createStore, sample } from "effector";
import { createBugFx } from "./newBug";
import { $initialReportForm, clearReport } from "./report";
import { updateBugApi } from "@/api/reports/bug";
import { Bug, UpdateBugParameters } from "@/types/bug";
import { RequestStates } from "@/const";

type UpdateBugParams = {
  reportId: number;
  bugId: number;
  bug: Bug;
};

export const updateBugFx = createEffect(
  ({ reportId, bugId, bug }: UpdateBugParams) => {
    const bugPayload = {
      id: bug.id,
      receive: bug.receive || null,
      expect: bug.expect || null,
      status: bug.status || null,
    };
    return updateBugApi(reportId, bugId, bugPayload);
  }
);

export const updateBugEvent = createEvent<UpdateBugParameters>();
export const updateBugApiEvent = createEvent<Bug>();
export const resetBug = createEvent<number>();

export const $initialBugsByBugId = createStore<Record<number, Bug> | null>(null)
  .on($initialReportForm, (_, report) =>
    report.bugs.reduce<Record<number, Bug>>((acc, bug) => {
      acc[bug.id] = bug;
      return acc;
    }, {})
  )
  .on(updateBugFx.done, (state, { result }) => {
    if (!result || state === null) return state;
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

export const $bugsByBugId = createStore<Record<number, Bug> | null>(null)
  .on($initialBugsByBugId, (_, bugs) => bugs)
  .on(updateBugEvent, (state, payload) => {
    const { id, receive, expect, status } = payload;
    if (!id || state === null) return state;
    const bug = state[id];

    const updatedBug = {
      ...bug,
      receive: receive ?? bug.receive,
      expect: expect ?? bug.expect,
      status: status ?? bug.status,
    };

    const initialBug = $initialBugsByBugId.getState()?.[id];
    if (!initialBug) return state;
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
    const initialBug = $initialBugsByBugId.getState()?.[bugId];

    if (!initialBug) return state;

    return {
      ...state,
      [bugId]: { ...initialBug, isChanged: false },
    };
  })
  .reset(clearReport);

export const $bugsIds = createStore<number[]>([])
  .on($initialBugsByBugId, (_, bugs) =>
    bugs ? Object.keys(bugs).map(Number) : []
  )
  .reset(clearReport);

export const $bugRequestState = createStore(RequestStates.IDLE);
$bugRequestState
  .on(updateBugFx.pending, (_, state) => {
    return state ? RequestStates.PENDING : RequestStates.DONE;
  })
  .on(updateBugFx.doneData, () => {
    return RequestStates.DONE;
  });

sample({
  source: updateBugApiEvent,
  fn: (bug) => ({
    reportId: bug.reportId,
    bugId: bug.id,
    bug,
  }),
  target: updateBugFx,
});
