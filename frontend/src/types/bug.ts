import { BugStatuses } from "@/const";
import { Attachment } from "./attachement";
import { Comment } from "./comment";
import { User } from "./user";

export type Bug = {
  id: number | null;
  reportId: number | null;
  receive?: string;
  expect?: string;
  creator?: User;
  createdAt?: Date;
  updatedAt?: Date;
  status: BugStatuses;
  attachments: Attachment[];
  comments: Comment[];
  isChanged: boolean;
};
