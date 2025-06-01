import { ReportStatuses } from "@/const";

export interface ReportPatchSocketResponse {
  title?: string | null;
  status?: ReportStatuses | null;
  responsibleUserId?: string | null;
  pastResponsibleUserId?: string | null;
  updatedAt: string;
}