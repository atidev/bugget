import { SocketEvent } from "@/webSocketApi/models";
import { socketEventReceived } from "@/store/socket";
import { SocketPayload } from "@/webSocketApi/models";
import { useEffect } from "react";

export function useSocketEvent<E extends SocketEvent>(
    type: E,
    handler: (payload: SocketPayload[E]) => void
) {
    useEffect(() => {
        const unsubscribe = socketEventReceived.watch((event) => {
            if (event.type === type) {
                handler(event.payload as SocketPayload[E]);
            }
        });
        return () => unsubscribe();
    }, [type, handler]);
}