import { API_URL } from "@/const";
import { extendedBasePath } from "@/api/basePath";

export const buildFullApiUrl = (path: string): string => {
  return buildFullUrl(API_URL, path);
};

export const buildFullAppUrl = (path: string): string => {
  return buildFullUrl("", path);
};

export const buildFullUrl = (baseUrl: string, path: string): string => {
  // Убираем завершающий слеш из baseUrl если он есть
  const baseUrlClean = baseUrl.endsWith("/") ? baseUrl.slice(0, -1) : baseUrl;
  const prefix = extendedBasePath || "";

  // Собираем URL: baseUrl + prefix + path
  const fullUrl = `${baseUrlClean}${prefix}/${path}`;

  return fullUrl;
};
