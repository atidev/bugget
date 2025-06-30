import React from "react";
import Header from "../Header/Header";

import "./Layout.css";
import Sidebar from "../Sidebar/Sidebar";

const Layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="flex flex-col h-full">
      <Header />
      <div className="grid grid-cols-[1fr_300px] grow">
        <div className="flex flex-col gap-4 px-6 py-8">{children}</div>
        <Sidebar />
      </div>
    </div>
  );
};

export default Layout;
