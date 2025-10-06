import { useLocation, useNavigate, useParams } from "react-router-dom";
import { clearReport } from "@/store/report";
import { setBreadcrumbs, $breadcrumbs } from "@/store/breadcrumbs";
import { useUnit } from "effector-react";
import { Search } from "lucide-react";
import Avatar from "../Avatar/Avatar";
import Breadcrumbs from "../Breadcrumbs/Breadcrumbs";
import { useEffect } from "react";

const HIDDEN_BUTTONS = {
  createReport: ["/reports"],
  search: ["/search"],
};

const reportsPageBreadcrumb = { label: "Репорты", path: "/" };

const Header = () => {
  const breadcrumbs = useUnit($breadcrumbs);
  const { reportId } = useParams();
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const breadcrumbs = [reportsPageBreadcrumb];

    if (location.pathname.startsWith("/reports")) {
      if (!reportId) {
        breadcrumbs.push({ label: "Новый репорт", path: `/reports` });
      } else {
        breadcrumbs.push({
          label: `Репорт #${reportId}`,
          path: `/reports/${reportId}`,
        });
      }
    }

    setBreadcrumbs(breadcrumbs);
  }, [reportId, location.pathname]);

  const isVisible = (button: keyof typeof HIDDEN_BUTTONS) =>
    !HIDDEN_BUTTONS[button].includes(location.pathname);

  return (
    <header className="h-16 justify-between bg-base-200 shadow-sm px-4 flex items-center">
      <div className="flex items-center">
        <Avatar />
        <Breadcrumbs breadcrumbs={breadcrumbs} />
      </div>
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
              navigate("/reports");
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
