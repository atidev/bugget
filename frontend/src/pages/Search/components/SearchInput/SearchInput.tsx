import { Search } from "lucide-react";
import { useState, useMemo } from "react";
import { updateQuery } from "@/storeObsolete/search";
import { debounce } from "throttle-debounce";

const SearchInput = () => {
  const [value, setValue] = useState("");

  const debouncedUpdateQuery = useMemo(
    () => debounce(300, (val: string) => updateQuery(val)),
    []
  );

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setValue(val);
    debouncedUpdateQuery(val);
  };

  return (
    <label className="flex w-full items-center gap-2 border border-gray-300 rounded-md px-3 py-2 focus-within:ring-2 focus-within:ring-blue-200 focus-within:border-transparent transition">
      <Search className="w-4 h-4 opacity-50" />
      <input
        type="search"
        className="grow bg-transparent focus:outline-none"
        placeholder="Начните вводить для поиска"
        value={value}
        onChange={handleChange}
      />
    </label>
  );
};

export default SearchInput;
