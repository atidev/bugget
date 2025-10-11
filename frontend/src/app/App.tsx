import "@/store";
import { useEffect, useMemo, useState } from "react";
import { BrowserRouter as Router, Outlet, useRoutes } from "react-router-dom";
import Layout from "@/components/Layout/Layout";
import Home from "@/pages/Home/Home";
import Report from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import Sidebar from "@/components/Sidebar/Sidebar";
import { authFx } from "@/store/user";
import "@/styles/tailwind.css";
import { loadExtensions } from "@/extensions/loader";
import { AppExtension, PatchableRouteObject } from "@/extensions/extension";
import { ApplyRoutesExtensions } from "@/extensions/routesApplyer";
import { BASE_PATH } from "@/const";
import ApiBaseBoot from "./ApiBaseBoot";

// Layout без сайдбара
const LayoutWrapper = () => (
  <Layout>
    <Outlet />
  </Layout>
);

// Layout с сайдбаром
const LayoutWithSidebarWrapper = () => (
  <Layout sidebar={<Sidebar />}>
    <Outlet />
  </Layout>
);

const baseRoutes: PatchableRouteObject[] = [
  {
    id: "root",
    path: "/",
    element: <LayoutWrapper />,
    children: [{ id: "dashboard", index: true, element: <Home /> }],
  },
  {
    id: "reports-root",
    path: "reports",
    element: <LayoutWithSidebarWrapper />,
    children: [
      {
        id: "reports-report",
        path: ":reportId",
        element: <Report />,
      },
      { id: "reports", index: true, element: <Report /> },
    ],
  },
  {
    id: "search-root",
    path: "search",
    element: <LayoutWrapper />,
    children: [{ id: "search", index: true, element: <Search /> }],
  },
];

function AppRoutes({ routes }: { routes: PatchableRouteObject[] }) {
  const element = useRoutes(routes);
  return element;
}

const App = () => {
  const [exts, setExts] = useState<AppExtension[]>([]);

  useEffect(() => {
    authFx();
  }, []);

  useEffect(() => {
    loadExtensions().then(setExts);
  }, []);

  const routes = useMemo(() => {
    const extra = exts.flatMap((e) => e.routes ?? []);
    return ApplyRoutesExtensions(baseRoutes, extra);
  }, [exts]);

  return (
    <Router basename={BASE_PATH}>
      <ApiBaseBoot />
      <AppRoutes routes={routes} />
    </Router>
  );
};

export default App;
