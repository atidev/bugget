import { BugStatuses, ReportStatuses } from "@/const";


export type User = {
  id: string;
  name: string;
};

export type AttachmentPayload = {
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

export type BugUpdateResponse = {
  id: number;
  receive: string;
  expect: string;
  status: BugStatuses;
};

export type BugCreatePayload = {
  receive: string;
  expect: string;
};

export type BugUpdatePayload = {
  id: number;
  receive: string | null;
  expect: string | null;
  status: BugStatuses | null;
};

export type Comment = {
  id: number;
  bugId: number;
  text: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
};

export type Report = {
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

export type CreateReportPayload = {
  title: string;
  responsibleId: string;
  participants?: User[];
  bugs?: BugCreatePayload[];
};


export type ReportPayload = {
  title: string;
  status: ReportStatuses;
  responsible?: User;
  responsibleId: string;
  creator?: User;
  createdAt?: string;
  updatedAt?: string;
  participants?: User[];
  bugs?: BugCreatePayload[];
};
