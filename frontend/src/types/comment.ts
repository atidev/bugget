import { User } from "./user";

export type Comment = {
  id: number;
  bugId: number;
  text: string;
  creator: User | null;
  createdAt: Date | null;
  updatedAt: Date | null;
};
