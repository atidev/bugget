import axios from "./axios";

export const fetchReportsSummary = async () => {
  try {
    const { data } = await axios.get("/v1/reports");
    return data;
  } catch (error) {
    console.error(error);
    return [];
  }
};
