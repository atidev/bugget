import { Attachment } from "./attachement";
import { User } from "./user";

export type Bug = {
  id: number;
  reportId: number;
  receive: string;
  expect: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
  status: number;
  attachments: Attachment[];
  comments: Comment[];
};
