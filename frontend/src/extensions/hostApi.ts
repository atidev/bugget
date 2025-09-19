import type { HostApi } from "./extension";
import { $user } from "@/store/user";

// реализация интерфейса доступа плагинов к сторам, эффектам, компонентам
export const hostApi: HostApi = {
  effector: {
    stores: {
      user: $user,
    },
  },
};
