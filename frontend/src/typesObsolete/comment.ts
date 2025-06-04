import { ObsoleteUserResponse } from "./user";

export type Comment = {
  id: number;
  bugId: number;
  text: string | null;
  creator: ObsoleteUserResponse | null;
  createdAt: Date | null;
  updatedAt: Date | null;
};
