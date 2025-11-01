import { bugStatusMap, reportStatusMap } from "@/const";
import { StatusMeta } from "@/types/status";
import { MessageCircleQuestion } from "lucide-react";

type EntityType = "bug" | "report";

const unknownStatus: StatusMeta = {
  title: "Неизвестно",
  bgColor: "bg-neutral",
  icon: MessageCircleQuestion,
  iconColor: "text-neutral",
};

export default function getStatusMeta(
  type: EntityType,
  status: number
): StatusMeta {
  if (type === "report") return reportStatusMap[status] ?? unknownStatus;
  if (type === "bug") return bugStatusMap[status] ?? unknownStatus;
  return unknownStatus;
}
