import axios from "../axios";

export const employeesAutocomplete = async (
  searchString: string,
  depth: number = 1
) => {
  try {
    const { data } = await axios.get(
      `/v1/employees/autocomplete/?searchString=${searchString}&depth=${depth}`
    );
    return data;
  } catch (error) {
    console.error(error);
    return {};
  }
};
