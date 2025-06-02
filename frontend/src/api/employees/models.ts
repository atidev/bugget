import { UserResponse } from "@/types/user";

export interface AutocompleteUsersResponse {
  employees: UserResponse[];
  total: number;
}
