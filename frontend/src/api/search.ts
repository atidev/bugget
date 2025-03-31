import axios from "./axios";
import type { SearchResponse } from "@/types/search";

export const searchReports = async (
  searchParams: string
): Promise<SearchResponse | void> => {
  try {
    const { data } = await axios.get(`/v1/reports/search?${searchParams}`);
    return data;
  } catch (error) {
    console.error(error);
  }
};
