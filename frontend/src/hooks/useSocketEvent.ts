import { useEffect } from "react";
import { SocketEvent, SocketPayload } from "@/types/socket";
import { socketEventReceived } from "@/store/socket";

/**
 * Хук для подписки на конкретное серверное событие.
 */
export function useSocketEvent<E extends SocketEvent>(
  type: E,
  handler: (payload: SocketPayload[E]) => void,
) {
  useEffect(() => {
    const unsub = socketEventReceived.watch((evt) => {
        if (evt.type === type) handler(evt.payload as SocketPayload[E]);
    });
    return () => unsub();
  }, [type, handler]);
}