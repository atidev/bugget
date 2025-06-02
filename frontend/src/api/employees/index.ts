import axios from "@/api/axios";
import { AutocompleteUsersResponse } from "./models";
import { UserResponse } from "@/types/user";

export const autocompleteUsers = async (
  searchString: string,
  skip: number = 0,
  take: number = 10,
  depth: number = 1
): Promise<AutocompleteUsersResponse> => {
  try {
    const { data } = await axios.get(`/v1/users/autocomplete`, {
      params: {
        searchString,
        skip,
        take,
        depth,
      },
    });
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};

export const listUsers = async (userIds: string[]): Promise<UserResponse[]> => {
  try {
    const { data } = await axios.post(`/v1/users/batch/list`, userIds);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};
