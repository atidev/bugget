import { Bug } from "@/types/bug";
import axios from "./axios";
import { BugUpdateRequest } from "@/types/requests";

export const createBugApi = async (
  reportId: number,
  createBugRequest: BugUpdateRequest
) => {
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
): Promise<Bug | null> => {
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
