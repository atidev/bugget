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
import { AppExtension } from "@/extensions/extension";
import { ApplyExtensions } from "@/extensions/applyer";
import { PatchableRouteObject } from "@/extensions/extension";

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

export function baseRoutes(): PatchableRouteObject[] {
  return [
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
}

function AppRoutes({ routes }: { routes: ReturnType<typeof baseRoutes> }) {
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
    return ApplyExtensions(baseRoutes(), extra);
  }, [exts]);

  return (
    <Router>
      <AppRoutes routes={routes} />
    </Router>
  );
};

export default App;
