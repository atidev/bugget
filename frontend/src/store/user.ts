import { createStore, createEffect } from "effector";
import { fetchAuth } from "../api/auth";
import { AuthUser } from "@/types/user";

export const authFx = createEffect(async () => {
  const data: Promise<AuthUser> = await fetchAuth();
  return data;
});

export const $user = createStore<AuthUser | null>(null).on(
  authFx.doneData,
  (_, user) => user
);
