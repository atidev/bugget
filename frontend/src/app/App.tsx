import "@/store/index";
import { useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "@/components/Layout/Layout";
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
      <Layout>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/reports" element={<Reports />} />
          <Route path="/new-reports" element={<NewReports />} />
          <Route path="/new-reports/:reportId" element={<NewReports />} />
          <Route path="/reports/:reportId" element={<Reports />} />
          <Route path="/search" element={<Search />} />
        </Routes>
      </Layout>
    </Router>
  );
};

export default App;
