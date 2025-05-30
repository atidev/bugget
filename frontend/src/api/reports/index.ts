import axios from "../axios";
import { CreateReportRequest, PatchReportRequest, PatchReportResponse, ReportResponse, ReportSummaryResponse } from "./models";

export const getReport = async (id: number): Promise<ReportResponse> => {
    try {
        const { data } = await axios.get(`/v2/reports/${id}`);
        return data;
    } catch (error) {
        console.error(error);
        throw error;
    }
};

export const createReport = async (request: CreateReportRequest): Promise<ReportSummaryResponse> => {
  try {
    const { data } = await axios.post<ReportSummaryResponse>("/v2/reports", request);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};

export const patchReport = async (id: number, request: PatchReportRequest): Promise<PatchReportResponse> => {
  try {
    const { data } = await axios.patch<PatchReportResponse>(`/v2/reports/${id}`, request);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};