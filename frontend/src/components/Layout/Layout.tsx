import React from "react";
import Header from "../Header/Header";

type LayoutProps = {
  children: React.ReactNode;
  sidebar?: React.ReactNode;
};

const Layout = ({ children, sidebar }: LayoutProps) => {
  return (
    <div className="flex flex-col h-full">
      <Header />
      <div
        className={`grow ${sidebar ? "grid grid-cols-[minmax(0,1fr)_300px]" : ""}`}
      >
        <main className="flex flex-col gap-4 px-6 py-8 max-w-[1024px] mx-auto w-full">
          {children}
        </main>
        {sidebar}
      </div>
    </div>
  );
};

export default Layout;
