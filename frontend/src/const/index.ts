import { StatusMeta } from "@/types/status";

export enum BugStatuses {
  IN_PROGRESS = 0,
  READY = 1,
}

export enum ReportStatuses {
  IN_PROGRESS = 0,
  READY = 1,
}

export enum RequestStates {
  IDLE = 0,
  PENDING = 1,
  DONE = 2,
  ERROR = 3,
}

export enum AttachmentTypes {
  RECEIVED_RESULT = 0,
  EXPECTED_RESULT = 1,
}

export const reportStatusMap: Record<number, StatusMeta> = {
  0: {
    title: "В работе",
    color: "badge-error",
    border: "border-error",
  },
  1: {
    title: "Решён",
    color: "badge-success",
    border: "border-success",
  },
};

export const bugStatusMap: Record<number, StatusMeta> = {
  0: {
    title: "Открыт",
    color: "badge-error",
    border: "border-error",
  },
  1: {
    title: "Исправлен",
    color: "badge-success",
    border: "border-success",
  },
};
