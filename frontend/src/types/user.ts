export type User = {
  name: string | null;
  id: string | null;
};

export type AuthUser = {
  name: string;
  id: string;
  teamId: string;
};
