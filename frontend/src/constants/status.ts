export type EntityType = "report" | "bug";

export interface StatusMeta {
  title: string;
  color:
    | "badge-success"
    | "badge-warning"
    | "badge-error"
    | "badge-info"
    | "badge-neutral";
  border: string;
}

export const reportStatusMap: Record<number, StatusMeta> = {
  0: {
    title: "В работе",
    color: "badge-warning",
    border: "border-warning",
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

export function getStatusMeta(type: EntityType, status: number): StatusMeta {
  if (type === "report") return reportStatusMap[status] ?? unknownStatus;
  if (type === "bug") return bugStatusMap[status] ?? unknownStatus;
  return unknownStatus;
}

const unknownStatus: StatusMeta = {
  title: "Неизвестно",
  color: "badge-neutral",
  border: "border-neutral",
};
