import { Bug } from "./bug";
import { User } from "./user";
import { ReportStatuses } from '../const/index';

export type Report = {
  id: number | null;
  title: string;
  status: ReportStatuses;
  responsible: User | null;
  responsibleId?: string;
  creator: User | null;
  createdAt: Date | null;
  updatedAt: Date | null;
  participants: User[] | never[];
  bugs: Bug[] | never[];
};
