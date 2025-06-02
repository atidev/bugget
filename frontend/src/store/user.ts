import { UserResponse } from "@/types/user";
import { createEffect, createStore } from "effector";
import { getAuth } from "@/api/auth";

export const authFx = createEffect(async () => {
  return await getAuth();
});

export const $user = createStore<UserResponse>({} as UserResponse).on(
  authFx.doneData,
  (_, user) => user
);
