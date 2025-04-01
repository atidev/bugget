import { Bug, ExistingBug, NewBug } from "./bug";
import { User } from "./user";
import { ReportStatuses } from "../const/index";

export type Report = {
  id: number | null;
  title: string;
  status: ReportStatuses;
  responsible: User | null;
  responsibleId: string;
  creator: User | null;
  createdAt: Date | null;
  updatedAt: Date | null;
  participants: User[];
  bugs: Bug[];
};

export type NewReport = {
  title: string;
  status: ReportStatuses;
  responsibleId: string;
  // todo: нужно убрать опциональность у след. параметров и назначать их при создании стора
  responsible?: User;
  creator?: User;
  participants?: User[] | never[];
  bugs: NewBug[];
};

export type ExistingReport = {
  id: number | null;
  title: string;
  status: ReportStatuses;
  responsible: User | null;
  responsibleId: string;
  creator: User | null;
  createdAt: Date | null;
  updatedAt: Date | null;
  participants: User[];
  bugs: ExistingBug[];
};
