import { createEvent } from "effector";
import { BugClientEntity } from "@/types/bug";

/**
 * Общие события для взаимодействия между сторами
 */

// Событие для установки багов (используется в bugs и attachments сторах)
export const setBugsEvent =
  createEvent<{ reportId: number; bugs: BugClientEntity[] }>();
