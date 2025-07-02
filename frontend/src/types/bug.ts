import { BugStatuses } from "@/const";
import { Attachment } from "./attachment";
import { Comment } from "./comment";

export type BugEntity = {
  id: number;
  reportId: number;
  receive: string | null;
  expect: string | null;
  creatorUserId: string;
  createdAt: string;
  updatedAt: string;
  status: BugStatuses;
  attachments: Attachment[] | null;
  comments: Comment[] | null;
};

export type BugFormData = {
  receive?: string;
  expect?: string;
};

export type BugUpdateData = {
  bugId: number;
  reportId: number;
  data: Partial<BugFormData & { status: BugStatuses }>;
};
