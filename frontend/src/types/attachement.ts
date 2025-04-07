export type Attachment = {
  id?: number;
  bugId: number;
  reportId: number;
  path?: string | null;
  createdAt?: string;
  attachType: number;
  file?: File;
};
