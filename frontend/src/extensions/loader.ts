import "@bugreport/host-sdk/init";
import { initExtensions } from "@bugreport/host-sdk/loader";
import type { AppExtension } from "@bugreport/host-sdk/types";
import { hostApi } from "./hostApi";

export async function loadExtensions(): Promise<AppExtension[]> {
  return initExtensions(hostApi, {
    debug: import.meta.env.DEV, // Enable debug logs in development
  });
}
