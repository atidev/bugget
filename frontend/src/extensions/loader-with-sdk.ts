/**
 * Extensions loader WITH SDK support
 * This file is only used when building with VITE_APP_EXTENSIONS
 * The SDK must be installed as a dependency for this to work
 */

// @ts-nocheck - SDK is optionally installed, only available when building with extensions

// Initialize shared dependencies first (side effect)
import "@bugget/host-sdk/init";
// Import the loader
import { initExtensions } from "@bugget/host-sdk/loader";
import type { AppExtension } from "./extension";
import type { HostApi } from "./extension";

export async function loadExtensionsWithSdk(
  hostApi: HostApi,
  debug: boolean
): Promise<AppExtension[]> {
  return initExtensions(hostApi, { debug });
}
