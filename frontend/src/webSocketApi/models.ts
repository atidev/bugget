import { ReportStatuses } from "@/const";

export enum SocketEvent {
    ReportParticipant = "ReceiveReportParticipant",
    ReportPatch = "ReceiveReportPatch",
}

export interface SocketPayload {
    [SocketEvent.ReportParticipant]: string; // participantUserId
    [SocketEvent.ReportPatch]: ReportPatchSocketResponse;
}

export interface ReportPatchSocketResponse {
    title: string | null;
    status: ReportStatuses | null;
    responsibleUserId: string | null;
    pastResponsibleUserId: string | null;
    updatedAt: string;
}