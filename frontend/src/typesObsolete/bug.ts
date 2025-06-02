import { BugStatuses } from "@/const";
import { Attachment } from "./attachement";
import { Comment } from "./comment";
import { ObsoleteUserResponse } from "./user";

export type Bug = {
  id: number;
  reportId: number;
  receive: string | null;
  expect: string | null;
  creator?: ObsoleteUserResponse;
  createdAt?: Date;
  updatedAt?: Date;
  status: BugStatuses;
  attachments?: Attachment[];
  comments?: Comment[];
  isChanged: boolean;
};

export type NewBug = {
  receive: string;
  expect: string;
  isReady?: boolean;
};

// параметры обновления существующего бага
export type UpdateBugParameters = {
  id: number;
  receive?: string;
  expect?: string;
  status?: BugStatuses;
};
