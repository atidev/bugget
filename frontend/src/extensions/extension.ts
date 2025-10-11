import { UserResponse } from "@/types/user";
import { Store } from "effector";
import { RouteObject } from "react-router-dom";
import React from "react";

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
      auth: Store<UserResponse | null>;
    };
  };
  // компоненты которые могут использоваться в расширениях
  components: {
    SidebarContainer: React.ComponentType<{ children: React.ReactNode }>;
    Layout: React.ComponentType<{
      children: React.ReactNode;
      sidebar?: React.ReactNode;
    }>;
  };
  // utils, etc
};

// статический ресурс плагина - маршруты
export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};
