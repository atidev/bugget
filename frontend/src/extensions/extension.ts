import { RouteObject } from "react-router-dom";

export type AppExtension = {
  id: string;
  routes?: PatchableRouteObject[];
};

export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};
