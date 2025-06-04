import { createStore, createEvent, createEffect, sample } from "effector";
import { createBugApi } from "@/apiObsolete/reports/bug";
import { createReportFx, clearReport } from "./report";
import { BugCreateRequest } from "@/apiObsolete/reports/models";
import { NewBug } from "@/typesObsolete/bug";

export const createBugFx = createEffect(
  ({ reportId, bug }: { reportId: number; bug: BugCreateRequest }) => {
    return createBugApi(reportId, bug);
  }
);

export const updateNewBug = createEvent<Partial<NewBug>>();

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
  bug: NewBug;
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
