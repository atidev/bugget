import React from "react";
import "./Sidebar.css";
import { $user } from "../../store/user";
import { useUnit } from "effector-react";

interface SidebarProps {
  isOpen: boolean;
}

const Sidebar: React.FC<SidebarProps> = () => {
  const [user] = useUnit([$user]);
  return (
    <div className="drawer-side">
      <label
        htmlFor="my-drawer"
        aria-label="close sidebar"
        className="drawer-overlay"
      ></label>
      <ul className="menu bg-base-200 text-base-content min-h-full w-80 p-4 sidebar">
        {/* Sidebar content here */}
        {user?.name &&
        <li>
          <a>{user.name}</a>
        </li>
        }
        <li>
          <a>В разработке</a>
        </li>
      </ul>
    </div>
  );
};

export default Sidebar;
