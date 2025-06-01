import { ReportPatchSocketResponse } from "@/webSocketApi/models";

export enum SocketEvent {
    ReportParticipant = "ReceiveReportParticipant",
    ReportPatch = "ReceiveReportPatch",
}

export type SocketPayload = {
    [SocketEvent.ReportPatch]: ReportPatchSocketResponse;
    [SocketEvent.ReportParticipant]: unknown; // TODO: добавить тип когда будет реализовано
}