import { createEffect, createStore } from "effector";
import { uploadAttachmentApi } from "@/api/reports/attachment";
import { $initialReportForm } from "./report";
import { Attachment } from "@/types/attachement";
import { Bug } from "@/types/bug";

export const uploadAttachmentFx = createEffect(async (params: Attachment) => {
  if (!params.file) return;
  const payload = {
    bugId: params.bugId,
    reportId: params.reportId,
    file: params.file,
    attachType: params.attachType,
  };
  return await uploadAttachmentApi(payload);
});

export const $attachmentsByBugId = createStore<Record<number, Attachment[]>>({})
  .on($initialReportForm, (_, report) => {
    if (!report?.bugs.length) return;
    return report.bugs.reduce((acc: Record<number, Attachment[]>, bug: Bug) => {
      if (bug.id) {
        acc[bug.id] = bug.attachments || [];
      }
      return acc;
    }, {});
  })
  .on(uploadAttachmentFx.doneData, (state, data) => {
    return {
      ...state,
      [data.bugId]: [...(state[data.bugId] || []), data],
    };
  });
