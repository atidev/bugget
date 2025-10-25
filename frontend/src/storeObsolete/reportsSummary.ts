import { createEffect, createStore, sample } from "effector";
import { $user } from "@/store/user";
import { Report } from "@/types/report";
import { listReports } from "@/api/reports";

export const loadReportsFx = createEffect(
  async (userId: string | null = null) => {
    const data = await listReports(userId);
    return data;
  }
);

export const $responsibleReports = createStore<Report[]>([]);

export const $participantReports = createStore<Report[]>([]);

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: Report[]) => {
    return reports.filter(
      (report: Report) => report.responsibleUserId === user?.id
    );
  },
  target: $responsibleReports,
});

sample({
  clock: loadReportsFx.doneData,
  source: $user,
  fn: (user, reports: Report[]) =>
    reports.filter((report: Report) => report.responsibleUserId !== user?.id),
  target: $participantReports,
});
