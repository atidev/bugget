import SortDropdown, { SortOption } from "@/components/SortDropdown/SortDropdown";
import SearchFilters from "./components/SearchFilters/SearchFilters";
import SearchResults from "./components/SearchResults/SearchResults";
import SearchInput from "./components/SearchInput/SearchInput";
import { useUnit } from "effector-react";
import { $sortField, $sortDirection, updateSortField, updateSortDirection, pageMounted } from "@/store/search";
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
  ];

  useEffect(() => {
    pageMounted();
  }, []);

  return (
    <div className="flex gap-6 p-4">
      <aside>
        <SearchFilters />
      </aside>

      <div className="flex-1">
        <div className="">
          <SearchInput />
          <div className="flex justify-end my-3">
            <SortDropdown
              options={options}
              value={sortField}
              direction={sortDirection}
              onChange={setSortField}
              onToggleDirection={() => setSortDirection(sortDirection === "asc" ? "desc" : "asc")}
            />
          </div>
        </div>
        <SearchResults />
      </div>
    </div>
  );
};

export default Search;
