import { createStore, createEffect } from "effector";
import { fetchAuth } from "../api/auth";


export const authFx = createEffect(async () => {
  const data: any = await fetchAuth();
  return data;
});

export const $user = createStore<any>(null).on(
  authFx.doneData,
  (_, user) => user
);