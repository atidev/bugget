import { UserResponse } from "@/types/user";
import { Store } from "effector";
import { RouteObject } from "react-router-dom";

/// интерфейсы для плагинов

// фабрикам реализуемая приложением-плагином
export type AppExtensionFactory = (
  host: HostApi // то что плагин может использовать на своей стороне
) => AppExtension | AppExtension[];

// интерфейс для плагинов
export type AppExtension = {
  id: string;
  routes?: PatchableRouteObject[]; // то что плагин может добавлять в приложение
  // navs, sidebars, etc
};

export type HostApi = {
  effector: {
    stores: {
      // доступ плагинов к сторам
      user: Store<UserResponse | null>;
    };
  };
  // components, utils, etc
};

// статический ресурс плагина - маршруты
export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};
