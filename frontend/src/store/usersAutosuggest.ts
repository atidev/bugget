import { createEvent, createStore, createEffect, sample } from 'effector';
import { debounce } from 'patronum';
import { employeesAutocomplete } from '@/api/users';

// типы
export type AutocompleteUser = {
  id: string;
  name: string;
};

// события
export const setSearchString = createEvent<string>();
export const clearSearch = createEvent();

// эффект запроса
export const fetchAutocompleteFx = createEffect(async (searchString: string) => {
  const response = await employeesAutocomplete(searchString);
  return response.employees;
});

// сторы
export const $searchString = createStore<string>("")
  .on(setSearchString, (_, value) => value)
  .reset(clearSearch);

export const $filteredItems = createStore<AutocompleteUser[]>([])
  .on(fetchAutocompleteFx.doneData, (_, items) => items)
  .reset(clearSearch);

// debounce-событие на 300ms
const debouncedSearch = debounce({
  source: setSearchString,
  timeout: 300,
});

// запускаем запрос с дебаунсом
sample({
  clock: debouncedSearch,
  filter: (search) => search.length > 0,
  target: fetchAutocompleteFx,
});