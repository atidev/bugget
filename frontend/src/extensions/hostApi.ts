import type { HostApi } from "./extension";
import { $user } from "@/store/user";
import { clearReport } from "@/store/report";
import { $usersStore } from "@/store/dashboard";
import { fetchReportsList } from "@/api/reports";
import { ReportStatuses } from "@/const";
import SidebarContainer from "@/components/Sidebar/SidebarContainer";
import HeaderContainer from "@/components/Header/HeaderContainer";
import Layout from "@/components/Layout/Layout";
import Report from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import ReportSidebar from "@/components/ReportSidebar/ReportSidebar";
import { DashboardContent } from "@/pages/Home/components";
import ReportCard from "@/components/ReportCard";

// реализация интерфейса доступа плагинов к сторам, эффектам, компонентам
export const hostApi: HostApi = {
  effector: {
    stores: {
      user: $user,
      usersStore: $usersStore,
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
    ReportSidebar,
    DashboardContent,
    ReportCard,
  },
  api: {
    fetchReportsList,
  },
  constants: {
    ReportStatuses,
  },
  types: {},
  utils: {},
};
