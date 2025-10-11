import { UserResponse } from "@/types/user";

export type AutocompleteUsersResponse = {
  users: UserResponse[];
  total: number;
};
