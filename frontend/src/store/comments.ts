import { createEffect, createEvent, createStore, sample } from "effector";
import { createComment, updateComment, deleteComment } from "@/api/comments";
import {
  getCommentAttachments,
  createCommentAttachment,
  deleteCommentAttachment,
} from "@/api/attachments";
import { Comment } from "@/types/comment";
import { Attachment } from "@/types/attachment";

/**
 * Сторы
 */
export const $commentsByBugId = createStore<Record<number, Comment[]>>({});

/**
 * Эффекты
 */

export const createCommentFx = createEffect<
  {
    reportId: number;
    bugId: number;
    text: string;
  },
  Comment,
  Error
>(async ({ reportId, bugId, text }) => {
  try {
    const result = await createComment(reportId, bugId, { text });
    return {
      id: result.id,
      bugId,
      text,
      creatorUserId: result.creator_user_id,
      createdAt: result.created_at,
      updatedAt: result.updated_at,
    };
  } catch (error) {
    console.error("❌ createCommentFx error:", error);
    throw error;
  }
});

export const updateCommentFx = createEffect<
  {
    reportId: number;
    bugId: number;
    commentId: number;
    text: string;
  },
  Comment,
  Error
>(async ({ reportId, bugId, commentId, text }) => {
  try {
    const result = await updateComment(reportId, bugId, commentId, { text });
    return {
      id: result.id,
      bugId,
      text,
      creatorUserId: result.creator_user_id,
      createdAt: result.created_at,
      updatedAt: result.updated_at,
    };
  } catch (error) {
    console.error("❌ updateCommentFx error:", error);
    throw error;
  }
});

export const deleteCommentFx = createEffect<
  {
    reportId: number;
    bugId: number;
    commentId: number;
  },
  void,
  Error
>(async ({ reportId, bugId, commentId }) => {
  try {
    await deleteComment(reportId, bugId, commentId);
  } catch (error) {
    console.error("deleteCommentFx error:", error);
    throw error;
  }
});

/**
 * События
 */

export const createCommentEvent = createEvent<{
  reportId: number;
  bugId: number;
  text: string;
}>();

export const updateCommentEvent = createEvent<{
  reportId: number;
  bugId: number;
  commentId: number;
  text: string;
}>();

export const deleteCommentEvent = createEvent<{
  reportId: number;
  bugId: number;
  commentId: number;
}>();

export const setCommentsByBugIdEvent = createEvent<
  {
    bugId: number;
    comments: Comment[];
  }[]
>();

export const fetchCommentAttachmentsFx = createEffect<
  { reportId: number; bugId: number; commentId: number },
  { bugId: number; commentId: number; attachments: Attachment[] }
>(async ({ reportId, bugId, commentId }) => {
  const res = await getCommentAttachments(reportId, bugId, commentId);
  const attachments: Attachment[] = res.map((attachment) => ({
    id: attachment.id,
    entityId: attachment.entityId,
    attachType: attachment.attachType,
    createdAt: attachment.createdAt,
    creatorUserId: attachment.creatorUserId,
    fileName: attachment.fileName,
    hasPreview: attachment.hasPreview,
  }));
  return { bugId, commentId, attachments };
});

export const createCommentAttachmentFx = createEffect<
  { reportId: number; bugId: number; commentId: number; file: File },
  { bugId: number; commentId: number; attachment: Attachment }
>(async ({ reportId, bugId, commentId, file }) => {
  const result = await createCommentAttachment(
    reportId,
    bugId,
    commentId,
    file
  );
  const attachment: Attachment = {
    id: result.id,
    entityId: result.entityId,
    attachType: result.attachType,
    createdAt: result.createdAt,
    creatorUserId: result.creatorUserId,
    fileName: result.fileName,
    hasPreview: result.hasPreview,
  };
  return { bugId, commentId, attachment };
});

export const deleteCommentAttachmentFx = createEffect<
  { reportId: number; bugId: number; commentId: number; attachmentId: number },
  { bugId: number; commentId: number; attachmentId: number }
>(async ({ reportId, bugId, commentId, attachmentId }) => {
  await deleteCommentAttachment(reportId, bugId, commentId, attachmentId);
  return { bugId, commentId, attachmentId };
});

/**
 * Логика
 */

$commentsByBugId
  .on(setCommentsByBugIdEvent, (state, allComments) => {
    console.log("setAllCommentsEvent", allComments);
    const newState = { ...state };
    allComments.forEach(({ bugId, comments }) => {
      newState[bugId] = comments;
    });
    return newState;
  })
  .on(createCommentFx.doneData, (state, comment) => {
    const bugId = comment.bugId;
    const existingComments = state[bugId] || [];
    return { ...state, [bugId]: [...existingComments, comment] };
  })
  .on(updateCommentFx.doneData, (state, updatedComment) => {
    const bugId = updatedComment.bugId;
    const existingComments = state[bugId] || [];
    return {
      ...state,
      [bugId]: existingComments.map((c) =>
        c.id === updatedComment.id ? updatedComment : c
      ),
    };
  })
  .on(deleteCommentEvent, (state, { bugId, commentId }) => {
    const existingComments = state[bugId] || [];
    return {
      ...state,
      [bugId]: existingComments.filter((c) => c.id !== commentId),
    };
  })
  .on(
    fetchCommentAttachmentsFx.doneData,
    (state, { bugId, commentId, attachments }) => {
      const existingComments = state[bugId] || [];
      return {
        ...state,
        [bugId]: existingComments.map((comment) =>
          comment.id === commentId ? { ...comment, attachments } : comment
        ),
      };
    }
  )
  .on(
    createCommentAttachmentFx.doneData,
    (state, { bugId, commentId, attachment }) => {
      const existingComments = state[bugId] || [];
      return {
        ...state,
        [bugId]: existingComments.map((comment) =>
          comment.id === commentId
            ? {
                ...comment,
                attachments: [...(comment.attachments || []), attachment],
              }
            : comment
        ),
      };
    }
  )
  .on(
    deleteCommentAttachmentFx.doneData,
    (state, { bugId, commentId, attachmentId }) => {
      const existingComments = state[bugId] || [];
      return {
        ...state,
        [bugId]: existingComments.map((comment) =>
          comment.id === commentId
            ? {
                ...comment,
                attachments: (comment.attachments || []).filter(
                  (a) => a.id !== attachmentId
                ),
              }
            : comment
        ),
      };
    }
  );

/**
 * Сэмплы
 */

sample({
  clock: createCommentEvent,
  target: createCommentFx,
});

sample({
  clock: updateCommentEvent,
  target: updateCommentFx,
});

sample({
  clock: deleteCommentEvent,
  target: deleteCommentFx,
});
