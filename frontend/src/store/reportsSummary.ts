import { createEffect, createStore, sample } from "effector";
import { fetchReportsSummary } from "@/api/reportsSummary";
import { $user } from "./user";
import { Report } from "@/types/report";

export const loadReportsFx = createEffect(async () => {
  const data: Report[] = await fetchReportsSummary();
  return data;
});

export const $responsibleReports = createStore<Report[]>([]);

export const $participantReports = createStore<Report[]>([]);

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: Report[]) => {
    return reports.filter((r: Report) => r.responsible.id === user?.id);
  },
  target: $responsibleReports,
});

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: Report[]) =>
    reports.filter((r: Report) => r.responsible.id !== user?.id),
  target: $participantReports,
});
