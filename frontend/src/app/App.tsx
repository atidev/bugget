import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "../components/Layout/Layout";
import Home from "../pages/Home/Home";
import Reports from "../pages/Report/Report";

import "../styles/tailwind.css";
import { useEffect } from "react";
import { authFx } from "../store/user";
import Search from "@/pages/Search/Search";

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
          <Route path="/search" element={<Search />} />
        </Routes>
      </Layout>
    </Router>
  );
};

export default App;
