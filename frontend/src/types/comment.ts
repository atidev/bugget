import { User } from "./user";

export type Comment = {
  id: number;
  bugId: number;
  text: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
};
