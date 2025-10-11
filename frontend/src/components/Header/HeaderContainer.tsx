import React from "react";

type HeaderContainerProps = {
  children: React.ReactNode;
};

/**
 * Базовый контейнер для хэдера
 */
const HeaderContainer = ({ children }: HeaderContainerProps) => {
  return (
    <div className="h-16 justify-between bg-base-200 shadow-sm px-4 flex items-center">
      {children}
    </div>
  );
};

export default HeaderContainer;
