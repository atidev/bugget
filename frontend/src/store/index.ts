import { sample, combine } from "effector";
import { $user } from "./user";
import {
  $creatorUserIdStore,
  $pastResponsibleUserIdStore,
  $responsibleUserIdStore,
  $reportPathStore,
  $reportIdStore,
  getReportFx,
} from "./report";
import { initSocketFx } from "./socket";
import { changeBugStatusEvent, updateBugDataEvent, setBugsEvent } from "./bugs";

const $src = combine({ user: $user, reportPath: $reportPathStore });

// заполнение сторов при открытии страницы создания репорта
sample({
  source: $src,
  filter: ({ reportPath, user }) =>
    reportPath === null && user.id !== undefined,
  fn: ({ user }) => user.id,
  target: [
    $creatorUserIdStore,
    $responsibleUserIdStore,
    $pastResponsibleUserIdStore,
  ],
});

// автоинициализация сокета при появлении репорта
sample({
  clock: $reportIdStore,
  filter: (id): id is number => id !== null,
  target: initSocketFx,
});

/**
 * Связи между bugs и report сторами
 */

// изменение статуса бага
sample({
  clock: changeBugStatusEvent,
  source: $reportIdStore,
  filter: (reportId) => reportId !== null,
  fn: (reportId, { bugId, status }) => ({
    bugId,
    reportId: reportId!,
    data: { status },
  }),
  target: updateBugDataEvent,
});

// загрузка багов при загрузке репорта
sample({
  clock: getReportFx.doneData,
  fn: (report) => ({
    reportId: report.id,
    bugs: report.bugs || [],
  }),
  target: setBugsEvent,
});
