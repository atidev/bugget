import { createEffect, createEvent, createStore, sample } from "effector";
import { $initialReportForm, clearReport } from "./report";
import { getCommentsApi, createCommentApi } from "@/api/reports/comment";
import { Comment } from "@/types/comment";

export const getCommentsFx = createEffect<
  {
    reportId: number;
    bugId: number;
  },
  Comment,
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
  Comment,
  Error
>(async ({ reportId, bugId, text }) => {
  return await createCommentApi(reportId, bugId, text);
});

export const newBugComments = createEvent<{
  bugId: number;
  comments: Comment[];
}>();

export const $commentsByBugId = createStore<Record<number, Comment[]>>({})
  .on($initialReportForm, (_, report) => {
    if (!report?.bugs.length) return;

    return report.bugs.reduce((acc, bug) => {
      // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore
      // todo: разобраться с типизацией
      acc[bug.id] = bug.comments || [];
      return acc;
    }, {});
  })
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
  })
  .reset(clearReport);

sample({
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  // todo fix ts-ignore
  clock: getCommentsFx.done,
  fn: ({ params, result }) => ({
    bugId: params.bugId,
    comments: result,
  }),
  target: newBugComments,
});
