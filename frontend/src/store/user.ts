import { User } from "@/types/user";
import { createEffect, createStore, combine } from "effector";
import { getAuth } from "@/api/auth";

export const authFx = createEffect(async () => {
    return await getAuth();
});

export const $user = createStore<User>({} as User).on(
    authFx.doneData,
    (_, user) => user
);

// Безопасный доступ к данным пользователя
export const $userData = combine({
    userId: $user.map(user => user.id || ""),
    isUserLoaded: $user.map(user => Boolean(user.id))
});