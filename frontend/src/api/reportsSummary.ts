import axios from "./axios";
import dataMock from "../mocks/reportsSummary.json";

export const fetchReportsSummary = async () => {
  try {
    const { data } = await axios.get("/v1/reports");
    return data;
  } catch (error) {
    console.error(error);
    return dataMock;
  }
};
