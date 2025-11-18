import { useUnit } from "effector-react";
import { $searchResult, $usersStore, loadMore } from "@/store/search";
import ReportCard from "@/components/ReportCard";
import { ChevronDown } from "lucide-react";

const SearchResults = () => {
  const [searchResult, usersStore] = useUnit([$searchResult, $usersStore]);

  return (
    <div className="flex flex-col gap-1">
      {searchResult?.reports?.map((report) => (
        <ReportCard key={report.id} report={report} usersStore={usersStore} />
      ))}
      {searchResult?.total > (searchResult?.reports?.length || 0) && (
        <button
          onClick={() => loadMore()}
          className="btn btn-outline btn-secondary"
        >
          <ChevronDown className="w-12 h-12" />
        </button>
      )}
    </div>
  );
};

export default SearchResults;
