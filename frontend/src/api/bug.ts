import { BugResponse } from "src/types/responses";
import axios from "./axios";
import { BugUpdateRequest } from "src/types/requests";

export const createBugApi = async (reportId: number, createBugRequest: any) => {
  try {
    const { data } = await axios.post(
      `/v1/reports/${reportId}/bugs`,
      createBugRequest
    );
    return data;
  } catch (error) {
    console.error("Ошибка при создании бага:", error);
    return null;
  }
};

export const updateBugApi = async (
  reportId: number,
  bugId: number,
  bug: BugUpdateRequest
): Promise<BugResponse | null> => {
  try {
    const { data } = await axios.put(
      `/v1/reports/${reportId}/bugs/${bugId}`,
      bug
    );
    return data;
  } catch (error) {
    console.error("Ошибка при обновлении бага:", error);
    return null;
  }
};
