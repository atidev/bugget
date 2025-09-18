export type CreateBugRequest = {
  receive?: string | null; // required, min length 1, max length 2048
  expect?: string | null; // required, min length 1, max length 2048
};

export type PatchBugRequest = {
  receive?: string | null; // required, min length 1, max length 2048
  expect?: string | null; // required, min length 1, max length 2048
  status?: number | null;
};

export type CreateBugResponse = {
  id: number;
  receive: string | null;
  expect: string | null;
  createdAt: string;
  updatedAt: string;
  creatorUserId: string;
  status: number;
};

export type PatchBugResponse = {
  id: number;
  receive: string | null;
  expect: string | null;
  updatedAt: string;
  status: number;
};
