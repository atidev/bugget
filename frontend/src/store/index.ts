import { sample, combine } from "effector";
import { $user } from "./user";
import {
  $creatorUserIdStore,
  $pastResponsibleUserIdStore,
  $responsibleUserIdStore,
  $reportPathStore,
  $reportIdStore,
} from "./report";
import { initSocketFx } from "./socket";

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
