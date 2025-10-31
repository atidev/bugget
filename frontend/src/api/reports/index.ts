import axios from "../axios";
import {
  CreateReportRequest,
  PatchReportRequest,
  PatchReportResponse,
  ReportResponse,
  CreateReportResponse,
  ListReportsResponse,
} from "./models";

export const fetchReport = async (id: number): Promise<ReportResponse> => {
  try {
    const { data } = await axios.get(`/v2/reports/${id}`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};

export const createReport = async (
  request: CreateReportRequest
): Promise<CreateReportResponse> => {
  try {
    const { data } = await axios.post<CreateReportResponse>(
      `/v2/reports`,
      request
    );
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};

export const patchReport = async (
  id: number,
  request: PatchReportRequest
): Promise<PatchReportResponse> => {
  try {
    const { data } = await axios.patch<PatchReportResponse>(
      `/v2/reports/${id}`,
      request
    );
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};

export const fetchReportsList = async (
  userId: string | null = null,
  teamId: string | null = null,
  reportStatuses: number[] | null = null,
  skip: number = 0,
  take: number = 10
): Promise<ListReportsResponse> => {
  try {
    const searchParams = new URLSearchParams();
    if (userId) searchParams.append("userId", userId);
    if (teamId) searchParams.append("teamId", teamId);
    if (reportStatuses) {
      for (const status of reportStatuses) {
        searchParams.append("reportStatuses", String(status));
      }
    }
    searchParams.append("skip", String(skip));
    searchParams.append("take", String(take));

    const { data } = await axios.get(`/v2/reports?${searchParams.toString()}`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};
