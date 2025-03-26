import axios from "./axios";

export const searchReports = async (searchParams: string) => {
  try {
    const { data } = await axios.get(`/v1/reports/search?${searchParams}`);
    return data;
  } catch (error) {
    console.error(error);
    return [];
  }
};
