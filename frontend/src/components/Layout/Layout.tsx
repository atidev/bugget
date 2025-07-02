import React from "react";
import Header from "../Header/Header";

import Sidebar from "../Sidebar/Sidebar";

const Layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="flex flex-col h-full">
      <Header />
      <div className="grid grid-cols-[minmax(0,1fr)_300px] grow">
        <main className="flex flex-col gap-4 px-6 py-8 max-w-[1024px] mx-auto w-full">
          {children}
        </main>
        <Sidebar />
      </div>
    </div>
  );
};

export default Layout;
