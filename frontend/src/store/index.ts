import { sample, combine, createStore } from "effector";

import { $bugsData } from "./bugs";
import {
  $localBugsStore,
  clearLocalBugEvent,
  promoteLocalBugToBackendEvent,
} from "./localBugs";
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

import { setBugsEvent } from "./commonEvents";

const $src = combine({ user: $user, reportPath: $reportPathStore });

// Мапа для сопоставления id c с бэка с clientId для промотированных багов
// Нужна, чтобы запоминать clientId бага, который был только на фронтенде, но появился на бэке
// clientId используем на ui как react key, чтобы не перерисовывать полностью компонент Bug
export const $clientIdMapStore = createStore<Record<number, number>>({})
  .on(promoteLocalBugToBackendEvent, (state, { tempClientId, backendBug }) => ({
    ...state,
    [backendBug.id]: tempClientId,
  }))
  .reset(clearLocalBugEvent);

// Стор для багов с бэка и локальных
export const $combinedBugsStore = combine(
  $bugsData,
  $localBugsStore,
  $clientIdMapStore,
  (bugsData, localBugs, clientIdMap) => {
    const { bugs, reportBugIds } = bugsData;

    const allBugIds = Object.values(reportBugIds).flat();

    // Обрабатываем backend баги
    const bugsFromStore = allBugIds.map((id: number) => {
      const bug = bugs[id];

      return bug.clientId
        ? bug
        : {
            ...bug,
            clientId: clientIdMap[bug.id] || bug.id,
          };
    });

    // Добавляем только локальные фронтовые баги
    const pendingLocals = localBugs.filter((bug) => bug.isLocalOnly);

    return bugsFromStore.concat(pendingLocals);
  }
);

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
    bugs: (report.bugs || []).map((bug) => ({
      ...bug,
      clientId: bug.id,
      isLocalOnly: false,
    })),
  }),
  target: setBugsEvent,
});

// очистка стора новых багов при смене репорта
sample({
  clock: $reportIdStore,
  target: clearLocalBugEvent,
});
