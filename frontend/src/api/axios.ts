import axios from "axios";
import {
  camelCase,
  snakeCase,
  isArray,
  isObject,
  mapKeys,
  mapValues,
} from "lodash";

const API_URL = window.env?.API_URL || import.meta.env.VITE_BASE_URL;

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

// Функция преобразования snake_case → camelCase
const snakeToCamel = (data: any): any => {
  if (isArray(data)) {
    return data.map(snakeToCamel);
  } else if (isObject(data)) {
    return mapValues(
      mapKeys(data, (_, key) => camelCase(key)),
      snakeToCamel
    );
  }
  return data;
};

// Функция преобразования camelCase → snake_case
const camelToSnake = (data: any): any => {
  if (isArray(data)) {
    return data.map(camelToSnake);
  } else if (isObject(data)) {
    return mapValues(
      mapKeys(data, (_, key) => snakeCase(key)),
      camelToSnake
    );
  }
  return data;
};

// Интерцептор ответа: преобразуем snake_case → camelCase
instance.interceptors.response.use((response) => {
  if (response.data) {
    response.data = snakeToCamel(response.data);
  }
  return response;
});

// Интерцептор запроса: преобразуем camelCase → snake_case
instance.interceptors.request.use((config) => {
  if (config.headers["Content-Type"] === "multipart/form-data") return config;
  if (config.data) {
    config.data = camelToSnake(config.data);
  }
  return config;
});

export default instance;
