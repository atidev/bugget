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
      usersStore: Store<Record<string, UserResponse>>;
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
      rightSidebar?: React.ReactNode;
      leftSidebar?: React.ReactNode;
      header?: React.ReactNode;
    }>;
    Report: React.ComponentType;
    Search: React.ComponentType;
    ReportSidebar: React.ComponentType;
    DashboardContent: React.ComponentType;
    ReportCard: React.ComponentType<{
      report: any;
      usersStore?: Record<string, any>;
      className?: string;
    }>;
  };
  // API для использования в расширениях
  api: {
    fetchReportsList: (
      userId: string | null,
      teamId: string | null,
      reportStatuses: number[] | null,
      skip?: number,
      take?: number
    ) => Promise<any>;
  };
  // Константы и типы
  constants: {
    ReportStatuses: typeof ReportStatuses;
  };
  types: {};
  // utils, etc
  utils: {};
};

// Типы для экспорта
export enum ReportStatuses {
  BACKLOG = 0,
  RESOLVED = 1,
  IN_PROGRESS = 2,
  REJECTED = 3,
}

// статический ресурс плагина - маршруты
export type PatchableRouteObject = RouteObject & {
  id?: string;
  children?: PatchableRouteObject[];
};
