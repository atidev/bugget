import axios from "../axios";
import dataMock from "../../mocks/employees.json";

export const employeesAutocomplete = async (searchString: string) => {
  try {
    const { data } = await axios.get(`/v1/employees/autocomplete/?searchString=${searchString}`);
    return dataMock;
  } catch (error) {
    console.error(error);
  }
};

