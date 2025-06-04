import { StatusMeta } from "@/typesObsolete/status";

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
