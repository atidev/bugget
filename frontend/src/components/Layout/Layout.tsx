import React from "react";
import Header from "../Header/Header";

type LayoutProps = {
  children: React.ReactNode;
  rightSidebar?: React.ReactNode;
  leftSidebar?: React.ReactNode;
  header?: React.ReactNode;
};

const Layout = ({
  children,
  rightSidebar,
  leftSidebar,
  header,
}: LayoutProps) => {
  const getGridClasses = () => {
    if (leftSidebar && rightSidebar) {
      return "grid grid-cols-[300px_minmax(0,1fr)_300px]";
    } else if (leftSidebar) {
      return "grid grid-cols-[300px_minmax(0,1fr)]";
    } else if (rightSidebar) {
      return "grid grid-cols-[minmax(0,1fr)_300px]";
    }
    return "";
  };

  return (
    <div className="flex flex-col h-full">
      {header || <Header />}
      <div className={`grow ${getGridClasses()}`}>
        {leftSidebar}
        <main className="flex flex-col gap-4 px-6 py-8 max-w-[1024px] mx-auto w-full">
          {children}
        </main>
        {rightSidebar}
      </div>
    </div>
  );
};

export default Layout;
