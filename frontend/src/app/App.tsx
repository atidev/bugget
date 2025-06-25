import "@/store";
import { useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "@/components/LayoutObsolette/Layout";
import LayoutNew from "@/components/Layout/Layout";
import Home from "@/pages/Home/Home";
import Reports from "@/pages/ReportObsolete/Report";
import NewReports from "@/pages/Report/Report";
import Search from "@/pages/Search/Search";
import { authFx } from "@/store/user";
import "@/styles/tailwind.css";

const App = () => {
  useEffect(() => {
    authFx();
  }, []);

  return (
    <Router>
      <Routes>
        <Route
          path="/"
          element={
            <Layout>
              <Home />
            </Layout>
          }
        />
        <Route
          path="/reports"
          element={
            <Layout>
              <Reports />
              <Route
                path="/reports/:reportId"
                element={
                  <Layout>
                    <Reports />
                  </Layout>
                }
              />
              <Route
                path="/search"
                element={
                  <Layout>
                    <Search />
                  </Layout>
                }
              />
            </Layout>
          }
        />
        <Route
          path="/new-reports"
          element={
            <LayoutNew>
              <NewReports />
            </LayoutNew>
          }
        />
        <Route
          path="/new-reports/:reportId"
          element={
            <LayoutNew>
              <NewReports />
            </LayoutNew>
          }
        />
      </Routes>
    </Router>
  );
};

export default App;
