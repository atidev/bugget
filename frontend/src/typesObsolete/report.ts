import { Bug, NewBug } from "./bug";
import { User } from "./user";
import { ReportStatuses } from "../const/index";

// модель стора со всеми данными по репорту
export type Report = {
  id: number | null;
  title: string;
  status: ReportStatuses;
  responsible: User | null;
  responsibleId: string | null;
  creator: User | null;
  createdAt: Date | null;
  updatedAt: Date | null;
  participants: User[] | null;
  bugs: Bug[];
};

// модель только с параметрами, необходимыми для создания нового репорта
export type NewReport = {
  title: string;
  status: ReportStatuses;
  responsibleId: string;
  bugs: NewBug[];
};

// модель для редактирования полей самого репорта с параметрами, вводимыми с фронта
// багов в этой модели нет, потому что интерфейс бага описан в bug.ts
export type ReportFormUIData = {
  id: number | null;
  title: string;
  status: ReportStatuses;
  responsible: User | null;
  participants: User[] | null;
};
