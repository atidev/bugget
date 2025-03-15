import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "../components/Layout/Layout";
import Home from "../pages/Home/Home";
import Reports from "../pages/Report/ReportPage";

import "../styles/tailwind.css";
import { useEffect } from "react";
import { authFx } from "../store/user";

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
          <Route path="/reports/:reportId" element={<Reports />} />
        </Routes>
      </Layout>
    </Router>
  );
};

export default App;
