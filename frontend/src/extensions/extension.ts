/**
 * Extension types
 * These types are used both in standalone mode and when @bugget/host-sdk is loaded
 */

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

export type AppExtensionFactory = (
  host: HostApi
) => AppExtension | AppExtension[];

// Global type declarations for window.env (used by extensions)
declare global {
  interface Window {
    env?: {
      API_URL?: string;
      BASE_PATH?: string;
      VITE_APP_EXTENSIONS?: string;
    };
  }
}
