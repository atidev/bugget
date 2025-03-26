import { createEffect, createStore } from "effector";
import { uploadAttachmentApi } from "../api/attachment";
import { $initialReportForm } from "./report";
import { Attachment } from "@/types/attachement";
import { Bug } from "@/types/bug";

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

export const $attachmentsByBugId = createStore<Record<number, Attachment[]>>({})
  .on($initialReportForm, (_, report) => {
    if (!report?.bugs.length) return;

    return report.bugs.reduce((acc: Record<number, Attachment[]>, bug: Bug) => {
      acc[bug.id] = bug.attachments || [];
      return acc;
    }, {});
  })
  .on(uploadAttachmentFx.doneData, (state, data) => {
    return {
      ...state,
      [data.bugId]: [...(state[data.bugId] || []), data],
    };
  });
