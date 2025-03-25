import { Report } from "./report";
export interface SearchParams {
    query?: string;
    reportStatuses?: number[];
    userId?: string;
    teamId?: string;
    sort?: string;
    skip?: number;
    take?: number;
}

export interface SearchResponse {
    reports: Report[];
    total: number;
}