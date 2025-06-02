import { AttachmentTypes, BugStatuses, ReportStatuses } from "@/const";

export type CreateReportRequest = {
  title: string; // required, min length 1, max length 128
};

export type CreateReportResponse = {
  id: number;
  title: string;
  status: ReportStatuses;
  responsibleUserId: string;
  pastResponsibleUserId: string;
  creatorUserId: string;
  createdAt: string;
  updatedAt: string;
};

export type ReportResponse = {
  id: number;
  title: string;
  status: ReportStatuses;
  responsibleUserId: string;
  pastResponsibleUserId: string;
  creatorUserId: string;
  createdAt: string;
  updatedAt: string;
  participantsUserIds: string[];
  bugs: BugResponse[] | null;
};

export type BugResponse = {
  id: number;
  reportId: number;
  receive: string | null;
  expect: string | null;
  creatorUserId: string;
  createdAt: string;
  updatedAt: string;
  status: BugStatuses;
  attachments: AttachmentResponse[] | null;
  comments: CommentResponse[] | null;
};

export type AttachmentResponse = {
  id: number;
  entityId: number;
  attachType: AttachmentTypes;
  createdAt: string;
  creatorUserId: string;
  fileName: string;
  hasPreview: boolean;
};

export type CommentResponse = {
  id: number;
  bugId: number;
  text: string;
  creatorUserId: string;
  createdAt: string;
  updatedAt: string;
  attachments: AttachmentResponse[] | null;
};

export type PatchReportRequest = {
  title?: string | null;
  status?: ReportStatuses | null;
  responsibleUserId?: string | null;
};

export type PatchReportResponse = {
  id: number;
  title: string;
  status: ReportStatuses;
  responsibleUserId: string;
  pastResponsibleUserId: string;
  updatedAt: string;
};
