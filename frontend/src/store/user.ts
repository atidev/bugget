import { UserResponse } from "@/types/user";
import { createEffect, createStore } from "effector";
import { fetchUser } from "@/api/users";

export const authFx = createEffect(async () => {
  return await fetchUser();
});

export const $user = createStore<UserResponse>({} as UserResponse).on(
  authFx.doneData,
  (_, user) => user
);
