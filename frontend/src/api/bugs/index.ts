import axios from "../axios";
import { CreateBugRequest, UpdateBugRequest, PatchBugResponse, BugResponse } from "./models";

export const createBug = async (reportId: number, request: CreateBugRequest): Promise<BugResponse> => {
    const { data } = await axios.post<BugResponse>(`/v2/reports/${reportId}/bugs`, request);
    return data;
};

export const updateBug = async (reportId: number, bugId: number, request: UpdateBugRequest): Promise<PatchBugResponse> => {
    const { data } = await axios.patch<PatchBugResponse>(`/v2/reports/${reportId}/bugs/${bugId}`, request);
    return data;
};

