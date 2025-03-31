import axios from "../axios";
import { AttachmentPayload } from "./models";

export const uploadAttachmentApi = async (params: AttachmentPayload) => {
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
