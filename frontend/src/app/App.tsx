import "@/store";
import { useEffect, useMemo, useState } from "react";
import { BrowserRouter as Router, Outlet, useRoutes } from "react-router-dom";
import Layout from "@/components/LayoutObsolette/Layout";
import LayoutNew from "@/components/Layout/Layout";
import Home from "@/pages/Home/Home";
import Reports from "@/pages/ReportObsolete/Report";
import NewReports from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import { authFx } from "@/store/user";
import "@/styles/tailwind.css";
import { loadExtensions } from "@/extensions/loader";
import { AppExtension, PatchableRouteObject } from "@/extensions/extension";
import { ApplyRoutesExtensions } from "@/extensions/routesApplyer";
import { BASE_PATH } from "@/const";

// Компонент-обертка для старого Layout
const LayoutWrapperObsolete = () => (
  <Layout>
    <Outlet />
  </Layout>
);

// Компонент-обертка для нового Layout
const LayoutWrapper = () => (
  <LayoutNew>
    <Outlet />
  </LayoutNew>
);

export const baseRoutes: PatchableRouteObject[] = [
  {
    id: "root",
    path: "/",
    element: <LayoutWrapperObsolete />,
    children: [
      { id: "dashboard", index: true, element: <Home /> },
      { id: "reports", path: "reports", element: <Reports /> },
      {
        id: "reports-report",
        path: "reports/:reportId",
        element: <Reports />,
      },
      { id: "search", path: "search", element: <Search /> },
    ],
  },
  {
    id: "new-reports-root",
    path: "/new-reports",
    element: <LayoutWrapper />,
    children: [
      { id: "new-reports", index: true, element: <NewReports /> },
      {
        id: "new-reports-report",
        path: ":reportId",
        element: <NewReports />,
      },
    ],
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
    loadExtensions().then((loadedExts) => {
      setExts(loadedExts);
    });
  }, []);

  const routes = useMemo(() => {
    const extra = exts.flatMap((e) => e.routes ?? []);
    return ApplyRoutesExtensions(baseRoutes, extra);
  }, [exts]);

  return (
    <Router basename={BASE_PATH}>
      <AppRoutes routes={routes} />
    </Router>
  );
};

export default App;
