import { UserResponse } from "@/types/user";
import { createEffect, createStore } from "effector";
import { fetchAuth } from "@/api/auth";

export const authFx = createEffect(async () => {
  return await fetchAuth();
});

export const $user = createStore<UserResponse>({} as UserResponse).on(
  authFx.doneData,
  (_, user) => user
);
