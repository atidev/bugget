import { BugStatuses } from "@/const";

import { Attachment } from "./attachment";
import { Comment } from "./comment";

export type BugClientEntity = {
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
  clientId: number;
  isLocalOnly: boolean;
};

export type BugFormData = {
  receive?: string;
  expect?: string;
  status?: BugStatuses;
};

export type BugUpdateData = {
  bugId: number;
  reportId: number;
  data: BugFormData;
};

export type ResultFieldTypes = "receive" | "expect";
