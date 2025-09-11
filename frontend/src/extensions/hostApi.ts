import type { HostApi } from "./extension";
import { $user } from "@/store/user";

export const hostApi: HostApi = {
  effector: {
    stores: {
      user: $user,
    },
  },
};
