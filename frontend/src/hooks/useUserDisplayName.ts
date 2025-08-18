import { useCallback } from "react";
import { useUnit } from "effector-react";
import { $usersStore } from "@/store/report";
import { $user } from "@/store/user";

export const useUserDisplayName = () => {
  const users = useUnit($usersStore);
  const currentUser = useUnit($user);

  return useCallback(
    (userId?: string, createdAt?: string) => {
      // 1) Точный матч: это текущий пользователь
      if (currentUser?.id && userId === currentUser.id) return "Вы";

      // 2) Пользователь найден в сторе по id
      if (userId && users[userId]?.name) return users[userId].name;

      // 3) Свежесозданный комментарий без userId: считаем, что это текущий пользователь
      if (currentUser?.id && (!userId || userId.trim() === "")) {
        if (createdAt) {
          const created = new Date(createdAt);
          if (
            !isNaN(created.getTime()) &&
            Date.now() - created.getTime() < 60 * 1000
          ) {
            return "Вы";
          }
        }
        // даже если времени нет, безопасный фолбек — "Вы", чтобы не было пустоты
        return "Вы";
      }

      // 4) Фолбек: показываем id, чтобы не было пустоты
      return userId || "Вы";
    },
    [users, currentUser?.id]
  );
};
