import { StatusMeta } from "@/typesObsolete/status";

export enum BugStatuses {
  ACTIVE = 0,
  ARCHIVED = 1,
  REJECTED = 2,
}

export enum ReportStatusesObsolete {
  IN_PROGRESS = 0,
  RESOLVED = 1,
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
  [ReportStatusesObsolete.IN_PROGRESS]: {
    title: "В работе",
    color: "badge-error",
    border: "border-error",
  },
  [ReportStatusesObsolete.RESOLVED]: {
    title: "Решён",
    color: "badge-success",
    border: "border-success",
  },
};

export const bugStatusMap: Record<number, StatusMeta> = {
  [BugStatuses.ACTIVE]: {
    title: "Открыт",
    color: "badge-error",
    border: "border-error",
  },
  [BugStatuses.ARCHIVED]: {
    title: "Исправлен",
    color: "badge-success",
    border: "border-success",
  },
};

export const API_URL = window.env?.API_URL || import.meta.env.VITE_BASE_URL;
export const BASE_PATH =
  window.env?.BASE_PATH || import.meta.env.VITE_BASE_PATH;

export enum BugResultTypes {
  RECEIVE = "receive",
  EXPECT = "expect",
}

export const justNowString = "только что";
export const yesterdayString = "вчера";
export const backInTimeString = "назад";
