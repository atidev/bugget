import "@/store";
import { useEffect, useMemo, useState } from "react";
import { BrowserRouter as Router, Outlet, useRoutes } from "react-router-dom";
import Layout from "@/components/Layout/Layout";
import Home from "@/pages/Home/Home";
import Report from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import { authFx } from "@/store/user";
import "@/styles/tailwind.css";
import { loadExtensions } from "@/extensions/loader";
import { AppExtension, PatchableRouteObject } from "@/extensions/extension";
import { ApplyRoutesExtensions } from "@/extensions/routesApplyer";
import { BASE_PATH } from "@/const";

const WrappedLayout = () => (
  <Layout>
    <Outlet />
  </Layout>
);

// Компоненты-обертки для разных случаев
const NewReportPage = () => <Report />;
const ReportPage = () => <Report />;

const baseRoutes: PatchableRouteObject[] = [
  {
    id: "root",
    path: "/",
    element: <WrappedLayout />,
    children: [
      { id: "dashboard", index: true, element: <Home /> },
      {
        id: "reports-report",
        path: "reports/:reportId",
        element: <ReportPage />,
      },
      { id: "reports", path: "reports", element: <NewReportPage /> },
      { id: "search", path: "search", element: <Search /> },
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
    loadExtensions().then(setExts);
  }, []);

  const routes = useMemo(() => {
    const extra = exts.flatMap((e) => e.routes ?? []);
    return ApplyRoutesExtensions(baseRoutes, extra);
  }, [exts]);

  return (
    <Router basename={BASE_PATH || "/"}>
      <AppRoutes routes={routes} />
    </Router>
  );
};

export default App;
