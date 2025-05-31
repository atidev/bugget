import { sample, combine  } from "effector";
import { $user } from "./user";
import {
    $creatorUserIdStore,
    $pastResponsibleUserIdStore,
    $responsibleUserIdStore,
    $reportPathStore
} from "./report";

const $src = combine({ user: $user, reportPath: $reportPathStore });

/**
 * Когда И (!) $isCreateMode === true
 *            user.id !== undefined
 * передаём user.id сразу в три стора-назначения
 */
sample({
  source: $src,
  filter: ({ reportPath, user }) => reportPath === null && user.id !== undefined,
  fn:   ({ user }) => user.id,
  target: [
    $creatorUserIdStore,
    $responsibleUserIdStore,
    $pastResponsibleUserIdStore,
  ],
});
