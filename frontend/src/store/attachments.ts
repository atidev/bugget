import { createEffect, createStore } from "effector";
import { uploadAttachmentApi } from "../api/attachment";
import { $initialReportForm } from "./report";
import { AttachmentResponse } from "src/types/responses";

export const uploadAttachmentFx = createEffect(
  async (params: {
    reportId: number;
    bugId: number;
    file: File;
    attachType: number;
  }) => {
    return await uploadAttachmentApi(params);
  }
);

export const $attachmentsByBugId = createStore<
  Record<number, AttachmentResponse[]>
>({})
  .on($initialReportForm, (_, report) =>
    report.bugs?.reduce((acc: Record<number, any[]>, bug: any) => {
      acc[bug.id] = bug.attachments || [];
      return acc;
    }, {})
  )
  .on(uploadAttachmentFx.doneData, (state, data) => {
    return {
      ...state,
      [data.bugId]: [...(state[data.bugId] || []), data],
    };
  });
