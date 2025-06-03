import { ReportStatuses } from "@/const";

export type PatchReportSocketResponse = {
  title?: string | null;
  status?: ReportStatuses | null;
  responsibleUserId?: string | null;
  pastResponsibleUserId?: string | null;
  updatedAt: string;
};

export type PatchBugSocketResponse = {
  receive?: string | null;
  expect?: string | null;
  status?: number | null;
};

export type CreateBugSocketResponse = {
  id: number;
  receive: string | null;
  expect: string | null;
  createdAt: string;
  updatedAt: string;
  creatorUserId: string;
  status: number;
};

export type CommentSocketResponse = {
  id: number;
  bugId: number;
  text: string;
  creatorUserId: string;
  createdAt: string;
  updatedAt: string;
};

export type AttachmentSocketResponse = {
  id: number;
  entityId: number;
  attachType: number;
  createdAt: string;
  creatorUserId: string;
  fileName: string;
  hasPreview: boolean;
};

export enum SocketEvent {
  ReportParticipant = "ReceiveReportParticipant",
  ReportPatch = "ReceiveReportPatch",
  BugPatch = "ReceiveBugPatch",
  BugCreate = "ReceiveBugCreate",
  CommentAttachmentCreate = "ReceiveCommentAttachmentCreate",
  BugAttachmentCreate = "ReceiveBugAttachmentCreate",
  CommentAttachmentOptimized = "ReceiveCommentAttachmentOptimized",
  BugAttachmentOptimized = "ReceiveBugAttachmentOptimized",
  CommentAttachmentDelete = "ReceiveCommentAttachmentDelete",
  BugAttachmentDelete = "ReceiveBugAttachmentDelete",
  CommentCreate = "ReceiveCommentCreate",
  CommentDelete = "ReceiveCommentDelete",
  CommentUpdate = "ReceiveCommentUpdate",
}

export type SocketPayload = {
  [SocketEvent.ReportPatch]: PatchReportSocketResponse;
  [SocketEvent.ReportParticipant]: string;
  [SocketEvent.BugPatch]: { bugId: number; patch: PatchBugSocketResponse };
  [SocketEvent.BugCreate]: CreateBugSocketResponse;
  [SocketEvent.CommentAttachmentCreate]: AttachmentSocketResponse;
  [SocketEvent.BugAttachmentCreate]: AttachmentSocketResponse;
  [SocketEvent.CommentAttachmentOptimized]: AttachmentSocketResponse;
  [SocketEvent.BugAttachmentOptimized]: AttachmentSocketResponse;
  [SocketEvent.CommentAttachmentDelete]: {
    id: number;
    entityId: number;
    attachType: number;
  };
  [SocketEvent.BugAttachmentDelete]: {
    id: number;
    entityId: number;
    attachType: number;
  };
  [SocketEvent.CommentCreate]: CommentSocketResponse;
  [SocketEvent.CommentDelete]: { bugId: number; commentId: number };
  [SocketEvent.CommentUpdate]: CommentSocketResponse;
};
