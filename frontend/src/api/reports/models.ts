import { BugStatuses, ReportStatuses } from "@/const";

export type User = {
  id: string;
  name: string;
};

export type AttachmentRequest = {
  bugId: number;
  reportId: number;
  file: File;
  attachType: number;
};

export type AttachmentResponse = {
  id: number;
  bugId: number;
  reportId: number;
  path: string;
  createdAt: string;
  attachType: number;
};

export type BugResponse = {
  id: number;
  reportId: number;
  receive: string;
  expect: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
  status: number;
  attachments: AttachmentResponse[];
  comments: Comment[];
};

export type BugCreateRequest = {
  receive: string;
  expect: string;
};

export type BugUpdateRequest = {
  id: number;
  receive: string | null;
  expect: string | null;
  status: BugStatuses | null;
};

export type CommentResponse = {
  id: number;
  bugId: number;
  text: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
};

export type ReportResponse = {
  id: number;
  title: string;
  status: ReportStatuses;
  responsible: User;
  creator: User;
  createdAt: string;
  updatedAt: string;
  participants: User[];
  bugs: BugResponse[];
};

export type CreateReportRequest = {
  title: string;
  responsibleId: string;
  participants?: User[];
  bugs?: BugCreateRequest[];
};

export type UpdateReportRequest = {
  title: string;
  status: ReportStatuses;
  responsible?: User;
  responsibleId: string;
  creator?: User;
  createdAt?: string;
  updatedAt?: string;
  participants?: User[];
  bugs?: BugCreateRequest[];
};
