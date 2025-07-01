import "@/store";
import { useEffect } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Outlet,
} from "react-router-dom";
import Layout from "@/components/LayoutObsolette/Layout";
import LayoutNew from "@/components/Layout/Layout";
import Home from "@/pages/Home/Home";
import Reports from "@/pages/ReportObsolete/Report";
import NewReports from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import { authFx } from "@/store/user";
import "@/styles/tailwind.css";

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

const App = () => {
  useEffect(() => {
    authFx();
  }, []);

  return (
    <Router>
      <Routes>
        {/* Роуты со старым Layout */}
        <Route path="/" element={<LayoutWrapperObsolete />}>
          <Route index element={<Home />} />
          <Route path="reports" element={<Reports />} />
          <Route path="reports/:reportId" element={<Reports />} />
          <Route path="search" element={<Search />} />
        </Route>

        {/* Роуты с новым Layout */}
        <Route path="/new-reports" element={<LayoutWrapper />}>
          <Route index element={<NewReports />} />
          <Route path=":reportId" element={<NewReports />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default App;
