import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { deleteBugAttachment, uploadAttachment } from "@/api/attachments";
import { AttachmentResponse } from "@/api/attachments/models";
import { Attachment } from "@/types/attachment";
import { setBugsEvent } from "./bugs"; // To populate attachments when bugs are set

// --- Сторы ---

// Стор для всех сущностей attachment, ключём по ID
export const $attachmentsStore = createStore<Record<number, Attachment>>({});

// Стор для маппинга id багов с attachment id
export const $bugAttachmentsStore = createStore<Record<number, number[]>>({});

// --- Эффекты ---

export const uploadAttachmentFx = createEffect<
  { reportId: number; bugId: number; attachType: number; file: File },
  { attachment: AttachmentResponse; bugId: number }
>(async ({ reportId, bugId, attachType, file }) => {
  try {
    const result = await uploadAttachment({
      reportId,
      bugId,
      attachType,
      file,
    });
    return { attachment: result, bugId };
  } catch (error) {
    console.error("❌ uploadAttachmentFx error:", error);
    throw error;
  }
});

export const deleteAttachmentFx = createEffect<
  { reportId: number; bugId: number; attachmentId: number },
  { bugId: number; attachmentId: number }
>(async ({ reportId, bugId, attachmentId }) => {
  try {
    await deleteBugAttachment(reportId, bugId, attachmentId);
    return { bugId, attachmentId };
  } catch (error) {
    console.error("❌ deleteAttachmentFx error:", error);
    throw error;
  }
});

// --- Ивенты ---

export const uploadAttachmentEvent = createEvent<{
  reportId: number;
  bugId: number;
  attachType: number;
  file: File;
}>();
export const deleteAttachmentEvent = createEvent<{
  reportId: number;
  bugId: number;
  attachmentId: number;
}>();

// --- Логика ---

// заполняем attachments при загрузке багов из основного репорта
$attachmentsStore.on(setBugsEvent, (state, { bugs }) => {
  const attachmentsById: Record<number, Attachment> = {};
  bugs.forEach((bug) => {
    if (bug.attachments) {
      bug.attachments.forEach((att) => {
        attachmentsById[att.id] = att;
      });
    }
  });
  return { ...state, ...attachmentsById };
});

$bugAttachmentsStore.on(setBugsEvent, (state, { bugs }) => {
  const bugAttachments: Record<number, number[]> = {};
  bugs.forEach((bug) => {
    // не перезаписываем существующие attachments для багов не в payload
    if (bug.attachments) {
      bugAttachments[bug.id] = bug.attachments.map((att) => att.id);
    }
  });
  return { ...state, ...bugAttachments };
});

$attachmentsStore.on(uploadAttachmentFx.doneData, (state, { attachment }) => {
  return {
    ...state,
    [attachment.id]: attachment,
  };
});

$bugAttachmentsStore.on(
  uploadAttachmentFx.doneData,
  (state, { attachment, bugId }) => {
    const currentAttachments = state[bugId] || [];
    return {
      ...state,
      [bugId]: [...currentAttachments, attachment.id],
    };
  }
);

$attachmentsStore.on(deleteAttachmentFx.doneData, (state, { attachmentId }) => {
  const newState = { ...state };
  delete newState[attachmentId];
  return newState;
});

$bugAttachmentsStore.on(
  deleteAttachmentFx.doneData,
  (state, { bugId, attachmentId }) => {
    const currentAttachments = state[bugId] || [];
    return {
      ...state,
      [bugId]: currentAttachments.filter((id) => id !== attachmentId),
    };
  }
);

// --- Samples ---

sample({
  clock: uploadAttachmentEvent,
  target: uploadAttachmentFx,
});

sample({
  clock: deleteAttachmentEvent,
  target: deleteAttachmentFx,
});

// --- Combined-сторы ---

export const $attachmentsData = combine(
  $attachmentsStore,
  $bugAttachmentsStore,
  (attachments, bugAttachments) => ({ attachments, bugAttachments })
);
