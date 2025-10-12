import type { HostApi } from "./extension";
import { $user } from "@/store/user";
import { clearReport } from "@/store/report";
import SidebarContainer from "@/components/Sidebar/SidebarContainer";
import HeaderContainer from "@/components/Header/HeaderContainer";
import Layout from "@/components/Layout/Layout";
import Report from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import Sidebar from "@/components/Sidebar/Sidebar";

// реализация интерфейса доступа плагинов к сторам, эффектам, компонентам
export const hostApi: HostApi = {
  effector: {
    stores: {
      user: $user,
    },
    events: {
      clearReport: () => clearReport(),
    },
  },
  components: {
    SidebarContainer,
    HeaderContainer,
    Layout,
    Report,
    Search,
    Sidebar,
  },
};
