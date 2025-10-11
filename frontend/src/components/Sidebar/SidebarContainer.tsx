import React from "react";

type SidebarContainerProps = {
  children: React.ReactNode;
};

/**
 * Базовый контейнер для сайдбара
 */
const SidebarContainer = ({ children }: SidebarContainerProps) => {
  return (
    <div className="flex flex-col gap-4 px-6 py-8 border-l border-base-content/30 rounded-l-sm">
      {children}
    </div>
  );
};

export default SidebarContainer;
