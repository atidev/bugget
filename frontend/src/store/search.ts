import {
  createEffect,
  createEvent,
  createStore,
  sample,
  combine,
} from "effector";
import { searchReports } from "@/api/searchReports/search";
import {
  SearchResponse,
  SearchRequestQueryParams,
} from "@/api/searchReports/models";
import { Team } from "@/types/team";
import { fetchUsers } from "@/api/users";
import { UserResponse } from "@/types/user";

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
export const searchPageOpened = createEvent();

export const updateQuery = createEvent<string>();
export const updateSortField = createEvent<string>();
export const updateSortDirection = createEvent<"asc" | "desc">();
export const updateStatuses = createEvent<number[] | null>();
export const updateUserFilter = createEvent<UserResponse | null>();
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

export const $userFilter = createStore<UserResponse | null>(null).on(
  updateUserFilter,
  (_, user) => user
);

export const $teamFilter = createStore<Team | null>(null).on(
  updateTeamFilter,
  (_, team) => team
);

export const $searchResult = createStore<SearchResponse>({} as SearchResponse)
  .on(searchFx.doneData, (_, payload) => payload)
  .reset(searchStarted);

// Загрузка пользователей
export const fetchUsersFx = createEffect<string[], UserResponse[]>(
  async (userIds) => {
    if (userIds.length === 0) return [];
    return await fetchUsers(userIds);
  }
);

// стор для хранения пользователей по ID
export const $usersStore = createStore<Record<string, UserResponse>>({}).on(
  fetchUsersFx.doneData,
  (state, users) => {
    const usersById = users.reduce(
      (acc, user) => {
        acc[user.id] = user;
        return acc;
      },
      {} as Record<string, UserResponse>
    );
    return { ...state, ...usersById };
  }
);

// получаем все уникальные id пользователей для загрузки
export const $allUserIdsStore = combine($searchResult, (searchResult) => {
  const allIds = new Set<string>();
  searchResult.reports?.forEach((report) => {
    if (report.responsibleUserId) allIds.add(report.responsibleUserId);
    if (report.creatorUserId) allIds.add(report.creatorUserId);
    report.participantsUserIds?.forEach((id) => {
      if (id) allIds.add(id);
    });
    report.bugs?.forEach((bug) => {
      if (bug.creatorUserId) allIds.add(bug.creatorUserId);
      bug.comments.forEach((comment) => {
        if (comment.creatorUserId) allIds.add(comment.creatorUserId);
      });
    });
  });
  return Array.from(allIds).filter(Boolean);
});

// загрузка пользователей после обновления результатов поиска
sample({
  clock: $searchResult.updates,
  source: $allUserIdsStore,
  filter: (userIds) => userIds.length > 0,
  target: fetchUsersFx,
});

sample({
  clock: searchStarted,
  target: searchFx,
});

// При изменении фильтров - запустить поиск
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
    updateUserFilter,
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
