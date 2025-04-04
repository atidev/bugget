import { useState, useRef, useMemo, useEffect } from "react";
import Avatar from "@/components/Avatar/Avatar";
import { debounce } from "throttle-debounce";

import "./Autosuggest.css";

type Props = {
  externalString?: string;
  onSelect: (entity: AutocompleteEntity | null) => void;
  autocompleteFn: (searchString: string) => Promise<AutocompleteEntity[]>;
};

type AutocompleteEntity = {
  display: string;
  id: string;
};

const Autosuggest = ({ externalString, onSelect, autocompleteFn }: Props) => {
  const [searchString, setSearchString] = useState(externalString);
  const [filteredItems, setFilteredItems] = useState<
    AutocompleteEntity[] | never[]
  >([]);
  const inputRef = useRef<null | HTMLInputElement>(null);

  const debouncedAutocompleteSearch = useMemo(
    () =>
      debounce(300, async (searchString: string) => {
        try {
          const data = await autocompleteFn(searchString);
          setFilteredItems(data);
        } catch (err) {
          console.error(err);
          setFilteredItems([]);
        }
      }),
    []
  );

  useEffect(() => {
    setSearchString(externalString ?? "");
    setFilteredItems([]);
  }, [externalString]);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    setSearchString(value);
    if (value) {
      debouncedAutocompleteSearch(value);
    }
  };

  const handleSelect = (item: AutocompleteEntity) => {
    setSearchString(item.display);
    onSelect(item);
    inputRef.current?.blur();
  };

  const handleItemClick = (
    event: React.SyntheticEvent,
    item: AutocompleteEntity
  ) => {
    event.preventDefault();
    handleSelect(item);
  };

  const clearInput = () => {
    setSearchString("");
    setFilteredItems([]);
    onSelect(null);
    inputRef.current?.focus();
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Backspace" && !searchString) {
      clearInput();
    }
  };

  return (
    <div className="dropdown">
      <input
        className="input input-bordered w-72"
        value={searchString ?? externalString}
        placeholder="Начните вводить"
        onChange={handleChange}
        tabIndex={0}
        onKeyDown={handleKeyDown}
      />
      {searchString && (
        <button
          className="clear-button btn btn-square btn-ghost bg-transparent hover:bg-transparent absolute right-2 top-1 shadow-none border-none h-6 w-6"
          onClick={clearInput}
          aria-label="Очистить"
        >
          <span>&times;</span>
        </button>
      )}
      {!!filteredItems?.length && (
        <ul
          tabIndex={0}
          className="users-list dropdown-content z-[2] menu p-2 shadow bg-base-200 rounded-box w-52 flex-nowrap overflow-auto absolute left-1/2 -translate-x-1/2"
        >
          {filteredItems.map((user) => (
            <li key={user.id} className="user-option">
              <Avatar width={2} />
              <a onClick={(event) => handleItemClick(event, user)}>
                {user.display}
              </a>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default Autosuggest;
