export type UserResponse = {
  id: string;
  name: string;
};

export type AttachmentResponse = {
  id: number;
  bugId: number;
  reportId: number;
  path: string;
  createdAt: string;
  attachType: number;
};

export type CommentResponse = {
  id: number;
  bugId: number;
  text: string;
  creator: UserResponse;
  createdAt: string;
  updatedAt: string;
};

export type BugResponse = {
  id: number;
  reportId: number;
  receive: string;
  expect: string;
  creator: UserResponse;
  createdAt: string;
  updatedAt: string;
  status: number;
  attachments: AttachmentResponse[];
  comments: CommentResponse[];
};

export type ReportResponse = {
  id: number;
  title: string;
  status: number;
  responsible: UserResponse;
  creator: UserResponse;
  createdAt: string;
  updatedAt: string;
  participants: UserResponse[];
  bugs: BugResponse[];
};
