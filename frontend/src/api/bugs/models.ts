export interface CreateBugRequest {
  receive?: string | null; // required, min length 1, max length 512
  expect?: string | null; // required, min length 1, max length 512
}

export interface UpdateBugRequest {
  receive?: string | null; // required, min length 1, max length 512
  expect?: string | null; // required, min length 1, max length 512
  status?: number | null;
}

export interface BugResponse {
  id: number;
  receive: string | null;
  expect: string | null;
  createdAt: string;
  updatedAt: string;
  creatorUserId: string;
  status: number;
}

export interface PatchBugResponse {
  id: number;
  receive: string | null;
  expect: string | null;
  updatedAt: string;
  status: number;
}
