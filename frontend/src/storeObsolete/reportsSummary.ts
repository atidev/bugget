import { createEffect, createStore, sample } from "effector";
import { fetchReportsSummary } from "@/apiObsolete/reports";
import { $user } from "@/store/user";
import { Report } from "@/typesObsolete/report";

export const loadReportsFx = createEffect(async () => {
  const data = await fetchReportsSummary();
  return data;
});

export const $responsibleReports = createStore<Report[]>([]);

export const $participantReports = createStore<Report[]>([]);

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: Report[]) => {
    return reports.filter(
      (report: Report) => report.responsible?.id === user?.id
    );
  },
  target: $responsibleReports,
});

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: Report[]) =>
    reports.filter((report: Report) => report.responsible?.id !== user?.id),
  target: $participantReports,
});
