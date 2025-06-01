import { AttachmentTypes } from "@/const";
import { AttachmentView } from "./models";
import axios from "../axios";

export const createAttachment = async (
    reportId: number,
    bugId: number,
    file: File,
    attachType: AttachmentTypes
): Promise<AttachmentView> => {
    const formData = new FormData();
    formData.append("file", file);
    const { data } = await axios.post<AttachmentView>(
        `/v2/reports/${reportId}/bugs/${bugId}/attachments?attachType=${attachType}`,
        formData,
        { headers: { "Content-Type": "multipart/form-data" } }
    );
    return data;
};

export const getAttachmentContent = async (reportId: number, bugId: number, attachmentId: number): Promise<Blob> => {
    const { data } = await axios.get<Blob>(
        `/v2/reports/${reportId}/bugs/${bugId}/attachments/${attachmentId}/content`,
        { responseType: "blob" }
    );
    return data;
};

export const getAttachmentPreview = async (reportId: number, bugId: number, attachmentId: number): Promise<Blob> => {
    const { data } = await axios.get<Blob>(
        `/v2/reports/${reportId}/bugs/${bugId}/attachments/${attachmentId}/content/preview`,
        { responseType: "blob" }
    );
    return data;
};

export const deleteAttachment = async (reportId: number, bugId: number, attachmentId: number): Promise<void> => {
    await axios.delete(`/v2/reports/${reportId}/bugs/${bugId}/attachments/${attachmentId}`);
}; 