import { UserResponse } from "@/types/user";
import { Store } from "effector";
import { RouteObject } from "react-router-dom";

export type HostApi = {
  effector: {
    stores: {
      user: Store<UserResponse | null>;
    };
  };
};

export type AppExtensionFactory = (
  host: HostApi
) => AppExtension | AppExtension[];

export type AppExtension = {
  id: string;
  routes?: PatchableRouteObject[];
};

export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};
