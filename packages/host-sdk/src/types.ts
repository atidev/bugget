/**
 * Type definitions for Bugreport host and extensions
 */

import type { Store } from "effector";
import type { RouteObject } from "react-router-dom";

/**
 * API provided by host to extensions
 */
export type HostApi = {
  effector: {
    stores: {
      auth: Store<UserResponse | null>;
    };
  };
};

/**
 * User response shape
 */
export type UserResponse = {
  id: string;
  name: string;
  teamId?: string | null;
  photoUrl?: string | null;
};

/**
 * Extension configuration
 */
export type AppExtension = {
  id: string;
  routes?: PatchableRouteObject[];
};

/**
 * Route object that can be patched by ID
 */
export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};

/**
 * Factory function that extensions export
 */
export type AppExtensionFactory = (host: HostApi) => AppExtension | AppExtension[];

/**
 * Shared dependencies exposed to extensions via window.__SHARED__
 */
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

/**
 * Global window interface with shared dependencies
 */
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

