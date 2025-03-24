import Dropdown from "@/components/Dropdown/Dropdown";
import { useState } from "react";


const SearchFilters = () => {

  const [statuses, setStatuses] = useState<number[] | null>(null);
  return (
    <>
      <div className="mb-4 text-lg font-bold text-gray-400">Поисковые фильтры</div>
      <Dropdown
        defaultValue={null}
        label="Статус репорта"
        multiple
        value={statuses}
        onChange={(value) => setStatuses(Array.isArray(value) ? value.filter(v => v !== null) : [value].filter(v => v !== null))}
        options={[
          { label: 'Активен', value: 0 },
          { label: 'Решён', value: 1 },
        ]}
      />
    </>
  );
};

export default SearchFilters;
