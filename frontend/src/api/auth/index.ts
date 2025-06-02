import axios from "@/api/axios";
import { UserResponse } from "@/types/user";

export const getAuth = async (): Promise<UserResponse> => {
  try {
    const { data } = await axios.get(`/v1/auth`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
};
