import { SocketEvent } from "@/webSocketApi/models";
import { SocketPayload } from "@/webSocketApi/models";

export type SocketSubscription<E extends SocketEvent> = {
    type: E;
    payload: SocketPayload[E];
};