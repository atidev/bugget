import React from "react";
import Header from "../Header/Header";

import "./Layout.css";
import Sidebar from "../Sidebar/Sidebar";

const Layout = ({ children }: { children: React.ReactNode }) => {
  // const [isSidebarOpen, setSidebarOpen] = useState(isMainPage);

  // useEffect(() => {
  //   if (!isMainPage) {
  //     setSidebarOpen(false);
  //   } else {
  //     setSidebarOpen(true);
  //   }
  // }, [isMainPage]);
  return (
    <div className="flex flex-col h-full">
      <Header />
      <div className="grid grid-cols-[1fr_300px] grow">
        <div className="flex flex-col gap-4 px-6 py-8">{children}</div>
        <Sidebar />
      </div>
    </div>

    // <div className="drawer">
    //   <input id="my-drawer" type="checkbox" className="drawer-toggle" />
    //   <div className="drawer-content">
    //     <Header />
    //     <main className="flex flex-col gap-4">{children}</main>
    //   </div>
    //   {/* <Sidebar isOpen={isSidebarOpen} /> */}
    // </div>
  );
};

export default Layout;
