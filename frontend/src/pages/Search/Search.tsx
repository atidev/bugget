import SortDropdown, {
  SortOption,
} from "@/components/SortDropdown/SortDropdown";
import SearchFilters from "./components/SearchFilters/SearchFilters";
import SearchResults from "./components/SearchResults/SearchResults";
import SearchInput from "./components/SearchInput/SearchInput";
import { useUnit } from "effector-react";
import {
  $sortField,
  $sortDirection,
  updateSortField,
  updateSortDirection,
  pageMounted,
} from "@/storeObsolete/search";
import { useEffect } from "react";

const Search = () => {
  const [sortField, sortDirection, setSortField, setSortDirection] = useUnit([
    $sortField,
    $sortDirection,
    updateSortField,
    updateSortDirection,
  ]);

  const options: SortOption[] = [
    { label: "Дата создания", value: "created" },
    { label: "Дата изменения", value: "updated" },
    { label: "Лучшее совпадение", value: "rank" },
  ];

  useEffect(() => {
    pageMounted();
  }, []);

  const onToggleDirectionHandler = () => {
    setSortDirection(sortDirection === "asc" ? "desc" : "asc");
  };

  return (
    <div className="flex gap-6 p-4">
      <div>
        <SearchFilters />
      </div>

      <div className="flex-1">
        <div>
          <SearchInput />
          <div className="flex justify-end my-3">
            <SortDropdown
              className="w-[245px]"
              options={options}
              value={sortField}
              direction={sortDirection}
              onChange={setSortField}
              onToggleDirection={onToggleDirectionHandler}
            />
          </div>
        </div>
        <SearchResults />
      </div>
    </div>
  );
};

export default Search;
