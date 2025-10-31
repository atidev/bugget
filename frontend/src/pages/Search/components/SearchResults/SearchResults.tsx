import { useUnit } from "effector-react";
import { $searchResult, $usersStore } from "@/store/search";
import ReportCard from "@/components/ReportCard";

const SearchResults = () => {
  const [searchResult, usersStore] = useUnit([$searchResult, $usersStore]);

  return (
    <div className="space-y-4">
      {searchResult?.reports?.map((report) => (
        <ReportCard key={report.id} report={report} usersStore={usersStore} />
      ))}
    </div>
  );
};

export default SearchResults;
