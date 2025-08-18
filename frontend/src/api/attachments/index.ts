import { AttachmentResponse } from "./models";
import axios from "../axios";

type UploadAttachmentParameters = {
  reportId: number;
  bugId: number;
  attachType: number;
  file: File;
};

export const uploadAttachment = async (params: UploadAttachmentParameters) => {
  try {
    const { reportId, bugId, file, attachType } = params;
    const formData = new FormData();
    formData.append("file", file);

    const { data } = await axios.post(
      `/v2/reports/${reportId}/bugs/${bugId}/attachments?attachType=${attachType}`,
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

export const deleteBugAttachment = async (
  reportId: number,
  bugId: number,
  attachmentId: number
): Promise<void> => {
  await axios.delete(
    `/v2/reports/${reportId}/bugs/${bugId}/attachments/${attachmentId}`
  );
};

export const createCommentAttachment = async (
  reportId: number,
  bugId: number,
  commentId: number,
  file: File
): Promise<AttachmentResponse> => {
  const formData = new FormData();
  formData.append("file", file);
  const { data } = await axios.post<AttachmentResponse>(
    `/v2/reports/${reportId}/bugs/${bugId}/comments/${commentId}/attachments`,
    formData,
    { headers: { "Content-Type": "multipart/form-data" } }
  );
  return data;
};

export const deleteCommentAttachment = async (
  reportId: number,
  bugId: number,
  commentId: number,
  attachmentId: number
): Promise<void> => {
  await axios.delete(
    `/v2/reports/${reportId}/bugs/${bugId}/comments/${commentId}/attachments/${attachmentId}`
  );
};

export const getCommentAttachments = async (
  reportId: number,
  bugId: number,
  commentId: number
): Promise<AttachmentResponse[]> => {
  const { data } = await axios.get<AttachmentResponse[]>(
    `/v2/reports/${reportId}/bugs/${bugId}/comments/${commentId}/`
  );
  return data;
};
