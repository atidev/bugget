import axios from "./axios";

export const fetchAuth = async () => {
  try {
    const { data } = await axios.get(`/v1/auth`);
    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
};

