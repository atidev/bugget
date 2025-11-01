import { useUnit } from "effector-react";
import { $searchResult, $usersStore } from "@/store/search";
import ReportCard from "@/components/ReportCard";

const SearchResults = () => {
  const [searchResult, usersStore] = useUnit([$searchResult, $usersStore]);

  return (
    <div className="flex flex-col gap-1">
      {searchResult?.reports?.map((report) => (
        <ReportCard key={report.id} report={report} usersStore={usersStore} />
      ))}
    </div>
  );
};

export default SearchResults;
