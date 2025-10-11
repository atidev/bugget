import type { HostApi } from "./extension";
import { $user } from "@/store/user";
import SidebarContainer from "@/components/Sidebar/SidebarContainer";
import Layout from "@/components/Layout/Layout";

// реализация интерфейса доступа плагинов к сторам, эффектам, компонентам
export const hostApi: HostApi = {
  effector: {
    stores: {
      auth: $user,
    },
  },
  components: {
    SidebarContainer,
    Layout,
  },
};
