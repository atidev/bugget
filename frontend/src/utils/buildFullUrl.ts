import { API_URL } from "@/const";
import { extendedBasePath } from "@/api/basePath";

export const buildFullUrl = (path: string): string => {
  // Убираем завершающий слеш из API_URL если он есть
  const baseUrl = API_URL.endsWith("/") ? API_URL.slice(0, -1) : API_URL;
  const prefix = extendedBasePath || "";

  // Собираем URL: baseUrl + prefix + path
  const fullUrl = `${baseUrl}${prefix}/${path}`;

  return fullUrl;
};
