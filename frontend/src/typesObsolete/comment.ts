import { User } from "./user";

export type Comment = {
  id: number;
  bugId: number;
  text: string | null;
  creator: User | null;
  createdAt: Date | null;
  updatedAt: Date | null;
};
