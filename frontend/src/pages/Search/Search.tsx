import SortDropdown, { SortOption } from "@/components/SortDropdown/SortDropdown";
import SearchFilters from "./components/SearchFilters/SearchFilters";
import SearchResults from "./components/SearchResults/SearchResults";
import SearchInput from "./components/SearchInput/SearchInput";
import { useState } from "react";


const Search = () => {
  const [sortField, setSortField] = useState("created");
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("desc");

  const options: SortOption[] = [
    { label: "Дата создания", value: "created" },
    { label: "Дата изменения", value: "updated" },
  ];

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
              onToggleDirection={() => setSortDirection(prev => prev === "asc" ? "desc" : "asc")}
            />
          </div>
        </div>
        <SearchResults />
      </div>
    </div>
  );
};

export default Search;
