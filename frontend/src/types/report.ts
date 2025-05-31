import { ReportStatuses } from "@/const";

export type Report = {
    id: number;
    title: string;
    status: ReportStatuses;
    responsibleUserId: string;
    pastResponsibleUserId: string;
    creatorUserId: string;
    createdAt: string;
    updatedAt: string;
}