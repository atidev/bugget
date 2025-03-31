import { Report } from "./report";
export type SearchParams = {
  query?: string;
  reportStatuses?: number[];
  userId?: string;
  teamId?: string;
  sort?: string;
  skip?: number;
  take?: number;
}

export type SearchResponse = {
  reports: Report[];
  total: number;
}
