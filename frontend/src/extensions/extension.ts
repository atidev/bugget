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
      user: Store<UserResponse | null>;
    };
    events: {
      // доступ плагинов к событиям
      clearReport: () => void;
    };
  };
  // компоненты которые могут использоваться в расширениях
  components: {
    SidebarContainer: React.ComponentType<{ children: React.ReactNode }>;
    HeaderContainer: React.ComponentType<{ children: React.ReactNode }>;
    Layout: React.ComponentType<{
      children: React.ReactNode;
      sidebar?: React.ReactNode;
      header?: React.ReactNode;
    }>;
    Report: React.ComponentType;
    Search: React.ComponentType;
    Sidebar: React.ComponentType;
  };
  // utils, etc
};

// статический ресурс плагина - маршруты
export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};
