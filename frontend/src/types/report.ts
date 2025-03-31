import { Bug } from "./bug";
import { User } from "./user";

export type Report = {
  id: number | null;
  title: string;
  status: number;
  responsible: User | null;
  responsibleId?: string;
  creator: User | null;
  createdAt: string;
  updatedAt: string;
  participants: User[] | never[];
  bugs: Bug[] | never[];
};
