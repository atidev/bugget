import { useUnit } from "effector-react";
import { $usersStore } from "@/store/report";
import { $user } from "@/store/user";

const youString = "Вы";

export const useUserDisplayName = (commentUserId?: string) => {
  const users = useUnit($usersStore);
  const currentUser = useUnit($user);

  if (currentUser?.id && commentUserId === currentUser.id) return youString;

  if (commentUserId && users[commentUserId]?.name)
    return users[commentUserId].name;

  return youString;
};
