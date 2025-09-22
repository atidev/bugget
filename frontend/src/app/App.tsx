import "@/store";
import { useEffect } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Outlet,
} from "react-router-dom";
import Layout from "@/components/Layout/Layout";
import Reports from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import Home from "@/pages/Home/Home";
import { authFx } from "@/store/user";
import "@/styles/tailwind.css";

// Компонент-обертка для нового Layout
const LayoutWrapper = () => (
  <Layout>
    <Outlet />
  </Layout>
);

const App = () => {
  useEffect(() => {
    authFx();
  }, []);

  return (
    <Router>
      <Routes>
        <Route path="/" element={<LayoutWrapper />}>
          <Route index element={<Home />} />
          <Route path="/reports" element={<Reports />} />
          <Route path="/reports/:reportId" element={<Reports />} />
          <Route path="/search" element={<Search />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default App;
