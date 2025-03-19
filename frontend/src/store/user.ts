import { createStore, createEffect } from "effector";
import { fetchAuth } from "../api/auth";
import { User } from "@/types/user";

export const authFx = createEffect(async () => {
  const data: Promise<User> = await fetchAuth();
  return data;
});

export const $user = createStore<User | null>(null).on(
  authFx.doneData,
  (_, user) => user
);