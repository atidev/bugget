import { AttachmentTypes } from "@/const";
import { AttachmentResponse } from "./models";
import axios from "../axios";

export const createBugAttachment = async (
  reportId: number,
  bugId: number,
  file: File,
  attachType: AttachmentTypes
): Promise<AttachmentResponse> => {
  const formData = new FormData();
  formData.append("file", file);
  const { data } = await axios.post<AttachmentResponse>(
    `/v2/reports/${reportId}/bugs/${bugId}/attachments?attachType=${attachType}`,
    formData,
    { headers: { "Content-Type": "multipart/form-data" } }
  );
  return data;
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
