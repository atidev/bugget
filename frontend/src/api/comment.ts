import axios from "./axios";

export const getCommentsApi = async (reportId: number, bugId: number) => {
  try {
    const { data } = await axios.get(
      `v1/reports/${reportId}/bugs/${bugId}/comments`
    );
    return data;
  } catch (error) {
    console.error("Ошибка при получении коммента:", error);
    return null;
  }
};

export const createCommentApi = async (
  reportId: number,
  bugId: number,
  text: string
) => {
  try {
    const { data } = await axios.post(
      `v1/reports/${reportId}/bugs/${bugId}/comments`,
      { text: text }
    );
    return data;
  } catch (error) {
    console.error("Ошибка при создании коммента:", error);
    return null;
  }
};
