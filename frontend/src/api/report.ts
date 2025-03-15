import axios from "./axios";
// import dataMock from "../mocks/report.json";

export const fetchReport = async (id: number) => {
  try {
    const { data } = await axios.get(`/v1/reports/${id}`);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};

export const createReport = async (report: any) => {
  try {
    const { data } = await axios.post(`/v1/reports`, report);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};

export const updateReport = async (updateRequest: any, reportId: number) => {
  try {
    const { data } = await axios.put(`/v1/reports/${reportId}`, updateRequest);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};
