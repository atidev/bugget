import axios from "./axios";

export const uploadAttachmentApi = async (params: {
  reportId: number;
  bugId: number;
  file: File;
  attachType?: number;
}) => {
  try {
    const { reportId, bugId, file, attachType = 1 } = params;
    const formData = new FormData();
    formData.append("file", file);

    // Важно: подставляем нужный URL и метод
    const { data } = await axios.post(
      `/v1/reports/${reportId}/bug/${bugId}/attachments?attachType=${attachType}`,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
    return data;
  } catch (error) {
    console.error("Ошибка при загрузке файла:", error);
    throw new Error("Не удалось загрузить файл");
  }
};
