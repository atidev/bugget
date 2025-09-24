import React from "react";
import { useLocation } from "react-router-dom";
import Header from "../Header/Header";

import Sidebar from "../Sidebar/Sidebar";

const Layout = ({ children }: { children: React.ReactNode }) => {
  const location = useLocation();

  const shouldShowSidebar = location.pathname !== "/";

  return (
    <div className="flex flex-col h-full">
      <Header />
      <div
        className={`grow ${
          shouldShowSidebar ? "grid grid-cols-[minmax(0,1fr)_300px]" : ""
        }`}
      >
        <main className="flex flex-col gap-4 px-6 py-8 max-w-[1024px] mx-auto w-full">
          {children}
        </main>
        {shouldShowSidebar && <Sidebar />}
      </div>
    </div>
  );
};

export default Layout;
