import axios from "axios";

import {
  convertObjectToCamel,
  convertObjectToSnake,
} from "@/utils/convertCases";

const API_URL = window.env?.API_URL || import.meta.env.VITE_BASE_URL;

const instance = axios.create({
  baseURL: API_URL,
  timeout: 10000,
});

let signalRConnectionId: string | null = null;
export const setSignalRConnectionId = (id: string | null) => {
  signalRConnectionId = id;
};

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
  if (config.headers["Content-Type"] !== "multipart/form-data") {
    config.data && (config.data = convertObjectToSnake(config.data));
  }

  if (signalRConnectionId) {
    config.headers["X-Signal-R-Connection-Id"] = signalRConnectionId;
  }

  return config;
});

export default instance;
