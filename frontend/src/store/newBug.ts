import { createStore, createEvent, createEffect, sample } from "effector";
import { createBugApi } from "../api/bug";
import { BugCreateRequest } from "@/types/requests";
import { createReportFx, clearReport } from "./report";

export const createBugFx = createEffect(
  async ({ reportId, bug }: { reportId: number; bug: BugCreateRequest }) => {
    return await createBugApi(reportId, bug);
  }
);

export const updateNewBug =
  createEvent<Partial<{ receive: string; expect: string }>>();

export const setExists = createEvent<boolean>();

export const $newBugStore = createStore<{
  receive: string;
  expect: string;
  isReady: boolean;
}>({
  receive: "",
  expect: "",
  isReady: false,
})
  .on(updateNewBug, (state, payload) => {
    const newState = { ...state, ...payload };
    return {
      ...newState,
      isReady: newState.receive.trim() !== "" && newState.expect.trim() !== "",
    };
  })
  .reset(createBugFx.done)
  .reset(createReportFx.doneData)
  .reset(clearReport);

export const $isExists = createStore<boolean>(false)
  .on(setExists, (_, isExists) => isExists)
  .reset(createBugFx.doneData)
  .reset(clearReport);

export const createBugEventByApi = createEvent<{
  reportId: number;
  bug: { receive: string; expect: string };
}>();

sample({
  source: createBugEventByApi,
  fn: ({ reportId, bug }) => ({
    reportId: reportId,
    bug: {
      receive: bug.receive,
      expect: bug.expect,
    } as BugCreateRequest,
  }),
  target: createBugFx,
});