import { createEffect, createEvent, createStore, sample } from "effector";
import { getCommentsApi, createCommentApi } from "../api/comment";
import { $initialReportForm } from "./report";

export const getCommentsFx = createEffect<
  {
    reportId: number;
    bugId: number;
  },
  any,
  Error
>(async ({ reportId, bugId }) => {
  return await getCommentsApi(reportId, bugId);
});

export const addCommentFx = createEffect<
  {
    reportId: number;
    bugId: number;
    text: string;
  },
  any,
  Error
>(async ({ reportId, bugId, text }) => {
  return await createCommentApi(reportId, bugId, text);
});

export const newBugComments = createEvent<{
  bugId: number;
  comments: any[];
}>();

export const $commentsByBugId = createStore<Record<number, any[]>>({})
  .on($initialReportForm, (_, report) =>
    report.bugs?.reduce((acc: Record<number, any[]>, bug: any) => {
      acc[bug.id] = bug.comments || [];
      return acc;
    }, {})
  )
  .on(newBugComments, (state, { bugId, comments }) => {
    return {
      ...state,
      [bugId]: comments,
    };
  })
  .on(addCommentFx.done, (state, { params, result }) => {
    return {
      ...state,
      [params.bugId]: [...(state[params.bugId] || []), result],
    };
  });

sample({
  clock: getCommentsFx.done,
  fn: ({ params, result }) => ({
    bugId: params.bugId,
    comments: result,
  }),
  target: newBugComments,
});
