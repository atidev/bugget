import axios from "axios";

import {
  convertObjectToCamel,
  convertObjectToSnake,
} from "@/utils/convertCases";
import { API_URL } from "@/const";

const instance = axios.create({
  baseURL: API_URL,
  timeout: 10000,
});

// Интерцептор ответа
instance.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error && error.response?.status >= 500) {
      const issueName = `${
        error.response.status
      } ${error.config?.method?.toUpperCase()} ${error.config?.url}`.trim();
      console.error(issueName);
    }
    return Promise.reject(error);
  }
);

// Интерцептор ответа: преобразуем snake_case → camelCase
instance.interceptors.response.use((response) => {
  if (response.data) {
    response.data = convertObjectToCamel(response.data);
  }
  return response;
});

// Интерцептор запроса: преобразуем camelCase → snake_case
instance.interceptors.request.use((config) => {
  if (config.headers["Content-Type"] === "multipart/form-data") return config;
  if (config.data) {
    config.data = convertObjectToSnake(config.data);
  }
  return config;
});

export default instance;
