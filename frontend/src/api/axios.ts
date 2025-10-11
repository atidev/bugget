import axios, { AxiosInstance } from "axios";

import {
  convertObjectToCamel,
  convertObjectToSnake,
} from "@/utils/convertCases";
import { API_URL, USERS_API_URL } from "@/const";
import { extendedBasePath } from "./basePath";

let signalRConnectionId: string | null = null;
export const setSignalRConnectionId = (id: string | null) => {
  signalRConnectionId = id;
};

// Функция для настройки response interceptors
const setupResponseInterceptors = (axiosInstance: AxiosInstance) => {
  // Интерцептор для обработки ошибок
  axiosInstance.interceptors.response.use(
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

  // Интерцептор для преобразования snake_case → camelCase
  axiosInstance.interceptors.response.use((response) => {
    if (response.data) {
      response.data = convertObjectToCamel(response.data);
    }
    return response;
  });
};

// Функция для настройки request interceptors
const setupRequestInterceptors = (axiosInstance: AxiosInstance) => {
  axiosInstance.interceptors.request.use((config) => {
    // динамически префиксуем только относительные url
    if (config.url && config.url.startsWith("/")) {
      const prefix = extendedBasePath || "";
      // избегаем двойных слэшей: "/abc" + "/v2" -> "/abc/v2"
      config.url = `${prefix}${config.url}`.replace(/\/{2,}/g, "/");
    }

    if (
      config.headers["Content-Type"] !== "multipart/form-data" &&
      config.data
    ) {
      config.data = convertObjectToSnake(config.data);
    }

    if (signalRConnectionId) {
      config.headers["X-Signal-R-Connection-Id"] = signalRConnectionId;
    }

    return config;
  });
};

// Функция для создания и настройки axios instance
const createAxiosInstance = (baseURL: string): AxiosInstance => {
  const instance = axios.create({
    baseURL,
    timeout: 10000,
  });

  setupResponseInterceptors(instance);
  setupRequestInterceptors(instance);

  return instance;
};

// Создаем основные instances
const instance = createAxiosInstance(API_URL);
const usersAxios = createAxiosInstance(USERS_API_URL);

export default instance;
export { usersAxios };
