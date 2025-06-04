import { UserResponse } from "@/types/user";

export type AutocompleteUsersResponse = {
  employees: UserResponse[];
  total: number;
};
