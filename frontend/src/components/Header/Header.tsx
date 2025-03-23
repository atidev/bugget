import "./Header.css";
import { useLocation, useNavigate } from "react-router-dom";
import { clearReport } from "@/store/report"; 

const Header = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const isNotCreatePage = location.pathname !== "/reports";
  return (
    <header className="h-16 justify-between bg-base-300 shadow-sm px-3 flex items-center">
      <label htmlFor="my-drawer" className="btn bg-base-200 drawer-button ">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          className="inline-block h-5 w-5 stroke-current sidebar-button"
        >
          {" "}
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="2"
            d="M4 6h16M4 12h16M4 18h16"
          ></path>{" "}
        </svg>
      </label>
      {isNotCreatePage && (
        <>
          <button
            className="btn-create-report"
            onClick={() => {
              clearReport();
              navigate("/reports");
            }}
          >
            Создать
          </button>
        </>
      )}
    </header>
  );
};

export default Header;
