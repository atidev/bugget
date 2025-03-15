import axios from "./axios";
import dataMock from "../mocks/report.json";

export const employeesAutocomplete = async (searchString: string) => {
  try {
    const { data } = await axios.get(`/v1/employees/autocomplete/?searchString=${searchString}`);
    return data;
  } catch (error) {
    console.error(error);
    return dataMock;
  }
};

