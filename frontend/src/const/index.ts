import { StatusMeta } from "@/types/status";

export enum BugStatuses {
  ACTIVE = 0,
  ARCHIVED = 1,
  REJECTED = 2,
}

export enum ReportStatuses {
  BACKLOG = 0,
  RESOLVED = 1,
  IN_PROGRESS = 2,
  REJECTED = 3,
}

export enum RequestStates {
  IDLE = 0,
  PENDING = 1,
  DONE = 2,
  ERROR = 3,
}

export enum AttachmentTypes {
  FACT = 0,
  EXPECT = 1,
  COMMENT = 2,
}

export const reportStatusMap: Record<number, StatusMeta> = {
  [ReportStatuses.IN_PROGRESS]: {
    title: "В работе",
    color: "badge-error",
    border: "border-error",
    bgColor: "bg-error",
  },
  [ReportStatuses.RESOLVED]: {
    title: "Решён",
    color: "badge-success",
    border: "border-success",
    bgColor: "bg-success",
  },
  [ReportStatuses.REJECTED]: {
    title: "Отклонён",
    color: "badge-info",
    border: "border-info",
    bgColor: "bg-info",
  },
  [ReportStatuses.BACKLOG]: {
    title: "Бэклог",
    color: "badge-neutral",
    border: "border-neutral",
    bgColor: "bg-neutral",
  },
};

export const bugStatusMap: Record<number, StatusMeta> = {
  [BugStatuses.ACTIVE]: {
    title: "Открыт",
    color: "badge-error",
    border: "border-error",
    bgColor: "bg-error",
  },
  [BugStatuses.ARCHIVED]: {
    title: "Исправлен",
    color: "badge-success",
    border: "border-success",
    bgColor: "bg-success",
  },
};

export const API_URL = window.env?.API_URL || import.meta.env.VITE_BASE_URL;
export const BASE_PATH =
  window.env?.BASE_PATH || import.meta.env.VITE_BASE_PATH;
export const USERS_API_URL = window.env?.USERS_API_URL || API_URL;

export enum BugResultTypes {
  RECEIVE = "receive",
  EXPECT = "expect",
}

export const justNowString = "только что";
export const yesterdayString = "вчера";
export const backInTimeString = "назад";
