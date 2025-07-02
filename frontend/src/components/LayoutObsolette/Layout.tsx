import React, { useEffect, useState } from "react";
import { useUnit } from "effector-react";
import { $breadcrumbs } from "../../storeObsolete/breadcrumbs";
import Sidebar from "../SidebarObsolette/Sidebar";
import Header from "../HeaderObsolette/Header";

import "./Layout.css";
import Breadcrumbs from "../Breadcrumbs/Breadcrumbs";

const Layout = ({ children }: { children: React.ReactNode }) => {
  const breadcrumbs = useUnit($breadcrumbs);

  const isMainPage = location.pathname === "/";

  const [isSidebarOpen, setSidebarOpen] = useState(isMainPage);

  useEffect(() => {
    if (!isMainPage) {
      setSidebarOpen(false);
    } else {
      setSidebarOpen(true);
    }
  }, [isMainPage]);
  return (
    <div className="drawer">
      <input id="my-drawer" type="checkbox" className="drawer-toggle" />
      <div className="drawer-content">
        <Header />
        <main className="flex flex-col gap-4 main-obsolete">
          <Breadcrumbs breadcrumbs={breadcrumbs} />
          {children}
        </main>
      </div>
      <Sidebar isOpen={isSidebarOpen} />
    </div>
  );
};

export default Layout;
