import axios from "../axios";
import {
  CreateBugRequest,
  PatchBugRequest,
  PatchBugResponse,
  CreateBugResponse,
} from "./models";

export const createBug = async (
  reportId: number,
  request: CreateBugRequest
): Promise<CreateBugResponse> => {
  const { data } = await axios.post<CreateBugResponse>(
    `/v2/reports/${reportId}/bugs`,
    request
  );
  return data;
};

export const updateBug = async (
  reportId: number,
  bugId: number,
  request: PatchBugRequest
): Promise<PatchBugResponse> => {
  const { data } = await axios.patch<PatchBugResponse>(
    `/v2/reports/${reportId}/bugs/${bugId}`,
    request
  );
  return data;
};
