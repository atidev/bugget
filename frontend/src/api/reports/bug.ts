import axios from "../axios";
import {
  BugUpdateResponse,
  BugCreateRequest,
  BugUpdateRequest,
} from "./models";
export const createBugApi = async (
  reportId: number,
  createBugRequest: BugCreateRequest
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
): Promise<BugUpdateResponse | null> => {
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
