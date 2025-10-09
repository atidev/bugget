declare global {
  interface Window {
    env?: {
      API_URL?: string;
      BASE_PATH?: string;
      VITE_APP_EXTENSIONS?: string;
    };
  }
}

export {};
