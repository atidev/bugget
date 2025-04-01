import { createStore, createEvent, createEffect, sample } from "effector";
import { createBugApi } from "@/api/reports/bug";
import { createReportFx, clearReport } from "./report";
import { BugCreateRequest } from "@/api/reports/models";

export const createBugFx = createEffect(
  async ({ reportId, bug }: { reportId: number; bug: BugCreateRequest }) => {
    return await createBugApi(reportId, bug);
  }
);

export const updateNewBug = createEvent<{
  receive?: string;
  expect?: string;
}>();

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
    const receive = newState.receive || "";
    const expect = newState.expect || "";
    return {
      ...newState,
      isReady: receive.trim() !== "" && expect.trim() !== "",
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
    },
  }),
  target: createBugFx,
});
