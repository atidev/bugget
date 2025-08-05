import { sample, combine } from "effector";

import { $bugsData, setBugsEvent } from "./bugs";
import { $localBugsStore, clearLocalBugEvent } from "./localBugs";
import {
  $creatorUserIdStore,
  $pastResponsibleUserIdStore,
  $responsibleUserIdStore,
  $reportPathStore,
  $reportIdStore,
  getReportFx,
} from "./report";
import { initSocketFx } from "./socket";
import { $user } from "./user";

const $src = combine({ user: $user, reportPath: $reportPathStore });

export const $allBugsStore = combine($bugsData, $localBugsStore);

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

// загрузка багов при загрузке репорта
sample({
  clock: getReportFx.doneData,
  fn: (report) => ({
    reportId: report.id,
    bugs: report.bugs || [],
  }),
  target: setBugsEvent,
});

// очистка стора новых багов при смене репорта
sample({
  clock: $reportIdStore,
  target: clearLocalBugEvent,
});
