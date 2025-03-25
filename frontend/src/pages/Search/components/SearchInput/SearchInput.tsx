import { Search } from "lucide-react";
import { useState, useEffect } from "react";
import { updateQuery } from "@/store/search";

const SearchInput = () => {
    const [value, setValue] = useState("");

    useEffect(() => {
        const timeout = setTimeout(() => {
            updateQuery(value);
        }, 300); // debounce

        return () => clearTimeout(timeout);
    }, [value]);

    return (
        <label className="flex w-full items-center gap-2 border border-gray-300 rounded-md px-3 py-2 focus-within:ring-2 focus-within:ring-blue-200 focus-within:border-transparent transition">
            <Search className="w-4 h-4 opacity-50" />
            <input
                type="search"
                className="grow bg-transparent focus:outline-none"
                placeholder="Начните вводить для поиска"
                value={value}
                onChange={(e) => setValue(e.target.value)}
            />
        </label>
    );
};

export default SearchInput;