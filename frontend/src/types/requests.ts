export type BugUpdateRequest = {
  receive?: string | null;
  expect?: string | null;
  status?: number | null;
};

export type BugCreateRequest = {
  receive: string;
  expect: string;
};
