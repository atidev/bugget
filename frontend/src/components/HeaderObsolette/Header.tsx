import { useLocation, useNavigate } from "react-router-dom";
import { clearReport } from "@/storeObsolete/report";
import { Search } from "lucide-react";

const HIDDEN_BUTTONS = {
  createReport: ["/reports"],
  search: ["/search"],
};

const Header = () => {
  const isVisible = (button: keyof typeof HIDDEN_BUTTONS) =>
    !HIDDEN_BUTTONS[button].includes(location.pathname);

  const navigate = useNavigate();
  const location = useLocation();

  return (
    <header className="h-16 justify-between bg-base-200 shadow-sm px-3 flex items-center">
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
      <div>
        {isVisible("search") && (
          <button
            className="btn bg-base-100"
            onClick={() => navigate("/search")}
          >
            <Search className="w-4 h-4" />
          </button>
        )}

        {isVisible("createReport") && (
          <button
            className="btn btn-primary ml-2 font-normal"
            onClick={() => {
              clearReport();
              navigate("/new-reports");
            }}
          >
            Новый репорт
          </button>
        )}
      </div>
    </header>
  );
};

export default Header;
