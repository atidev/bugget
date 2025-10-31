import { createStore, createEvent } from "effector";
import { Breadcrumb } from "@/types/ui";

export const setBreadcrumbs = createEvent<Breadcrumb[]>();
export const $breadcrumbs = createStore<Breadcrumb[]>([]).on(
  setBreadcrumbs,
  (_, breadcrumbs) => breadcrumbs
);
