import { BugStatuses } from "const";

export interface BugStore {
  id: number;
  reportId: number;
  receive: string;
  expect: string;
  status: BugStatuses;
  isChanged: boolean;
}
