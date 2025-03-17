import { useState, useRef, useMemo } from "react";
import Avatar from "../../../../components/Avatar/Avatar";
import { debounce } from "throttle-debounce";
import { employeesAutocomplete } from "../../../../api/users";

import "./UsersAutosuggest.css";
import { User } from "@/types/user";

type Props = {
  onSelect: (id: string, name: string) => void;
  externalString?: string;
};

const UsersAutosuggest = ({ onSelect, externalString }: Props) => {
  const [searchString, setSearchString] = useState(externalString);
  const [filteredItems, setFilteredItems] = useState<User[] | never[]>([]);
  const inputRef = useRef<null | HTMLInputElement>(null);

  const debouncedAutocompleteSearch = useMemo(
    () =>
      debounce(300, async (searchString: string) => {
        try {
          const response = await employeesAutocomplete(searchString);
          // здесь, внутри коллбэка, мы имеем доступ к результатам
          setFilteredItems(response.employees);
        } catch (err) {
          console.error(err);
          setFilteredItems([]);
        }
      }),
    []
  );

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    setSearchString(value);
    if (value) {
      debouncedAutocompleteSearch(value);
    }
  };

  const handleUserSelect = (item: User) => {
    setSearchString(item.fullName);
    onSelect(item.userId, item.fullName);
    inputRef.current?.blur();
  };

  const handleItemClick = (event: React.SyntheticEvent, item: User) => {
    event.preventDefault();
    handleUserSelect(item);
  };

  return (
    <div className="dropdown">
      <input
        className="input input-bordered w-72"
        value={searchString ?? externalString}
        placeholder="Начните вводить"
        onChange={handleChange}
        tabIndex={0}
      />
      {!!filteredItems?.length && (
        <ul
          tabIndex={0}
          className="users-list dropdown-content z-[2] menu p-2 shadow bg-base-200 rounded-box w-52 flex-nowrap overflow-auto"
        >
          {filteredItems.map((user) => {
            return (
              <li key={user.userId} className="user-option">
                <Avatar width={2} />
                <a onClick={(event) => handleItemClick(event, user)}>
                  {user.fullName}
                </a>
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
};

export default UsersAutosuggest;
