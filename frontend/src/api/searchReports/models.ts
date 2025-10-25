import { BugStatuses, ReportStatuses } from "@/const";

export type User = {
  id: string | null;
  name: string | null;
};

export type AttachmentResponse = {
  id: number;
  bugId: number;
  reportId: number;
  path: string | null;
  createdAt: string;
  attachType: number;
};

export type BugResponse = {
  id: number;
  reportId: number;
  receive: string | null;
  expect: string | null;
  creator: User;
  createdAt: string;
  updatedAt: string;
  status: BugStatuses;
  attachments: AttachmentResponse[];
  comments: CommentResponse[];
};

export type CommentResponse = {
  id: number;
  bugId: number;
  text: string | null;
  creator: User;
  createdAt: string;
  updatedAt: string;
};

export type ReportResponse = {
  id: number;
  title: string | null;
  status: ReportStatuses;
  responsible: User;
  creator: User;
  createdAt: string;
  updatedAt: string;
  participants: User[] | null;
  bugs: BugResponse[] | null;
};

export type SearchResponse = {
  reports: ReportResponse[];
  total: number;
};

export type SearchRequestQueryParams = {
  query?: string;
  reportStatuses?: number[];
  userId?: string;
  teamId?: string;
  sort?: string;
  skip?: number;
  take?: number;
};
