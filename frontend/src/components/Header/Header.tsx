import { useLocation, useNavigate, useParams } from "react-router-dom";
import { clearReport } from "@/storeObsolete/report";
import { setBreadcrumbs } from "@/storeObsolete/breadcrumbs";
import { useUnit } from "effector-react";
import { $breadcrumbs } from "../../storeObsolete/breadcrumbs";
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

  useEffect(() => {
    setBreadcrumbs([
      reportsPageBreadcrumb,
      !reportId
        ? { label: "Новый репорт", path: `/reports` }
        : {
            label: `Репорт #${reportId}`,
            path: `/reports/${reportId}`,
          },
    ]);
  }, [reportId]);

  const isVisible = (button: keyof typeof HIDDEN_BUTTONS) =>
    !HIDDEN_BUTTONS[button].includes(location.pathname);

  const navigate = useNavigate();
  const location = useLocation();

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
