import { Bug } from "./bug";
import { User } from "./user";

export type Report = {
  id: number;
  title: string;
  status: number;
  responsible: User;
  creator: User;
  createdAt: string;
  updatedAt: string;
  participants: User[];
  bugs: Bug[];
};
