import Dropdown from "@/components/Dropdown/Dropdown";
import { updateStatuses, $statuses } from "@/store/search";
import { useUnit } from "effector-react";

const SearchFilters = () => {

  const statuses = useUnit($statuses)
  return (
    <>
      <div className="mb-4 text-lg font-bold">Поисковые фильтры</div>
      <Dropdown
        defaultValue={null}
        label="Статус репорта"
        multiple
        value={statuses}
        onChange={(value) => {
          const list = Array.isArray(value) ? value : [value];
          updateStatuses(list.filter(v => v !== null));
        }}
        options={[
          { label: 'В работе', value: 0 },
          { label: 'Решён', value: 1 },
        ]}
      />
    </>
  );
};

export default SearchFilters;