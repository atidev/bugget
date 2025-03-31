

export type User = {
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

export type BugResponse = {
  id: number;
  reportId: number;
  receive: string;
  expect: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
  status: number;
  attachments: AttachmentResponse[];
  comments: Comment[];
};

export type Bug = {
  id: number;
  receive: string | null;
  expect: string | null;
  status: number | null;
};

export type BugCreateRequest = {
  receive: string;
  expect: string;
};

export type BugUpdateRequest = {
  id: number;
  receive: string | null;
  expect: string | null;
  status: number | null;
};

export type BugUpdateResponse = {
  receive: string | null;
  expect: string | null;
  status: number | null;
};

export type Comment = {
  id: number;
  bugId: number;
  text: string;
  creator: User;
  createdAt: string;
  updatedAt: string;
};

export type Report = {
  id: number;
  title: string;
  status: number;
  responsible: User;
  creator: User;
  createdAt: string;
  updatedAt: string;
  participants: User[];
  bugs: Bug[];
};

export type CreateReportPayload = {
  title: string;
  responsibleId: string;
  participants?: User[];
  bugs?: BugCreateRequest[];
};


export type ReportPayload = {
  title: string;
  status: number;
  responsible?: User;
  responsibleId: string;
  creator?: User;
  createdAt?: string;
  updatedAt?: string;
  participants?: User[];
  bugs?: BugCreateRequest[];
};
