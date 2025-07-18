import { createEffect, createEvent, createStore, sample } from "effector";
import { searchReports } from "@/apiObsolete/reports/search";
import {
  SearchResponse,
  SearchRequestQueryParams,
  User,
} from "@/apiObsolete/reports/models";
import { $user } from "@/store/user";
import { Team } from "@/typesObsolete/team";

export const searchFx = createEffect(
  async (params: SearchRequestQueryParams) => {
    const searchParams = new URLSearchParams();
    if (params.query) searchParams.append("query", params.query);
    if (params.sort) searchParams.append("sort", params.sort);
    if (params.userId) searchParams.append("userId", params.userId);
    if (params.teamId) searchParams.append("teamId", params.teamId);
    if (params.skip != null) searchParams.append("skip", String(params.skip));
    if (params.take != null) searchParams.append("take", String(params.take));
    if (params.reportStatuses) {
      for (const status of params.reportStatuses) {
        searchParams.append("reportStatuses", String(status));
      }
    }
    return await searchReports(searchParams.toString());
  }
);

export const searchStarted = createEvent<SearchRequestQueryParams>();
export const pageMounted = createEvent();

export const updateQuery = createEvent<string>();
export const updateSortField = createEvent<string>();
export const updateSortDirection = createEvent<"asc" | "desc">();
export const updateStatuses = createEvent<number[] | null>();
export const updateUserFilter = createEvent<User | null>();
export const updateTeamFilter = createEvent<Team | null>();

export const $query = createStore<string>("").on(updateQuery, (_, q) => q);
export const $sortField = createStore<string>("created").on(
  updateSortField,
  (_, field) => field
);
export const $sortDirection = createStore<"asc" | "desc">("desc").on(
  updateSortDirection,
  (_, direction) => direction
);
export const $statuses = createStore<number[] | null>(null).on(
  updateStatuses,
  (_, s) => s
);

export const $userFilter = createStore<User | null>(null)
  .on($user, (_, user) => (user ? { id: user.id, name: user.name } : null))
  .on(updateUserFilter, (_, user) => user);

export const $teamFilter = createStore<Team | null>(null).on(
  updateTeamFilter,
  (_, team) => team
);

export const $searchResult = createStore<SearchResponse>({} as SearchResponse)
  .on(searchFx.doneData, (_, payload) => payload)
  .reset(searchStarted);

sample({
  clock: searchStarted,
  target: searchFx,
});

sample({
  source: {
    query: $query,
    sortField: $sortField,
    sortDirection: $sortDirection,
    reportStatuses: $statuses || undefined,
    userFilter: $userFilter,
    teamFilter: $teamFilter,
  },
  clock: pageMounted,
  fn: ({
    query,
    sortField,
    sortDirection,
    reportStatuses,
    userFilter,
    teamFilter,
  }) => ({
    query,
    sort: `${sortField}_${sortDirection}`,
    reportStatuses: reportStatuses ?? undefined,
    userId: userFilter?.id ?? undefined,
    teamId: teamFilter?.id ?? undefined,
    skip: 0,
    take: 100,
  }),
  target: searchStarted,
});

sample({
  source: {
    query: $query,
    sortField: $sortField,
    sortDirection: $sortDirection,
    reportStatuses: $statuses,
    userFilter: $userFilter,
    teamFilter: $teamFilter,
  },
  clock: [
    $query.updates,
    $sortField.updates,
    $sortDirection.updates,
    $statuses.updates,
    $userFilter.updates,
    $teamFilter.updates,
  ],
  fn: ({
    query,
    sortField,
    sortDirection,
    reportStatuses,
    userFilter,
    teamFilter,
  }) => ({
    query,
    sort: `${sortField}_${sortDirection}`,
    reportStatuses: reportStatuses ?? undefined,
    userId: userFilter?.id ?? undefined,
    teamId: teamFilter?.id ?? undefined,
    skip: 0,
    // TODO пагинация
    take: 100,
  }),
  target: searchStarted,
});
