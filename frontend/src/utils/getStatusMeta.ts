import { bugStatusMap, reportStatusMap } from "@/const";
import { StatusMeta } from "@/types/status";

type EntityType = "bug" | "report";

const unknownStatus: StatusMeta = {
  title: "Неизвестно",
  color: "badge-neutral",
  border: "border-neutral",
};

export default function getStatusMeta(
  type: EntityType,
  status: number
): StatusMeta {
  if (type === "report") return reportStatusMap[status] ?? unknownStatus;
  if (type === "bug") return bugStatusMap[status] ?? unknownStatus;
  return unknownStatus;
}
