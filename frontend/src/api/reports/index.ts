import axios from "../axios";
import { CreateReportRequest, UpdateReportRequest } from "./models";

export const fetchReport = async (id: number) => {
  try {
    const { data } = await axios.get(`/v1/reports/${id}`);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};

export const createReport = async (report: CreateReportRequest) => {
  try {
    const { data } = await axios.post(`/v1/reports`, report);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};

export const updateReport = async (
  updateRequest: UpdateReportRequest,
  reportId: number
) => {
  try {
    const { data } = await axios.put(`/v1/reports/${reportId}`, updateRequest);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};

export const fetchReportsSummary = async () => {
  try {
    const { data } = await axios.get("/v1/reports");
    return data;
  } catch (error) {
    console.error(error);
    return [];
  }
};
