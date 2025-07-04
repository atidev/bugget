import { AttachmentTypes } from "@/const";

export type Attachment = {
  id: number;
  entityId: number;
  attachType: AttachmentTypes;
  createdAt: string;
  creatorUserId: string;
  fileName: string;
  hasPreview: boolean;
};
