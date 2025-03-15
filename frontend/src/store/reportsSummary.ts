import { createEffect, createStore, sample } from "effector";
import { fetchReportsSummary } from "../api/reportsSummary";
import { $user } from "./user";
import { ReportResponse } from "src/types/responses";

export const loadReportsFx = createEffect(async () => {
  const data: ReportResponse[] = await fetchReportsSummary();
  return data;
});

export const $responsibleReports = createStore<ReportResponse[]>([]);

export const $participantReports = createStore<ReportResponse[]>([]);

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: ReportResponse[]) => {
    return reports.filter((r: ReportResponse) => r.responsible.id === user?.id);
  },
  target: $responsibleReports,
});

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: ReportResponse[]) =>
    reports.filter((r: ReportResponse) => r.responsible.id !== user?.id),
  target: $participantReports,
});
