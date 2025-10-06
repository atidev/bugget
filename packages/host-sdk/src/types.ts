import type { Store } from "effector";
import type { RouteObject } from "react-router-dom";

export type HostApi = {
  effector: {
    stores: {
      auth: Store<UserResponse | null>;
    };
  };
};

export type UserResponse = {
  id: string;
  name: string;
  teamId?: string | null;
  photoUrl?: string | null;
};

export type AppExtension = {
  id: string;
  routes?: PatchableRouteObject[];
};

export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};

export type AppExtensionFactory = (host: HostApi) => AppExtension | AppExtension[];

export type SharedDependencies = {
  react: typeof import("react") & {
    jsxRuntime: typeof import("react/jsx-runtime");
  };
  reactDOM: typeof import("react-dom");
  reactRouterDOM: typeof import("react-router-dom");
  effector: typeof import("effector");
  effectorReact: typeof import("effector-react");
  axios: typeof import("axios");
};

declare global {
  interface Window {
    __SHARED__?: SharedDependencies;
    __SHARED_VERSION__?: string;
    env?: {
      API_URL?: string;
      BASE_PATH?: string;
      VITE_APP_EXTENSIONS?: string;
    };
  }
}

