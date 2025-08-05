import { createEvent, createStore, sample } from "effector";

import { CreateBugResponse } from "@/api/bugs/models";
import { BugResultTypes, BugStatuses } from "@/const";
import { BugFormData, ResultFieldTypes, BugClientEntity } from "@/types/bug";

import { createBugFx } from "./bugs";
import { $user } from "./user";

export type LocalBug = {
  receive: string;
  expect: string;
  hasCreatedOnBackend: boolean;
  reportId: number | null;
  clientId: number | null;
};

/**
 * События
 */
export const setLocalBugReportEvent =
  createEvent<{ reportId: number; clientId: number }>();
export const updateLocalBugFieldEvent =
  createEvent<{ clientId: number; field: ResultFieldTypes; value: string }>();

export const clearLocalBugEvent = createEvent<void>();
export const createLocalBugEvent = createEvent<{ reportId: number }>();
export const removeLocalBugEvent = createEvent<{ clientId: number }>();

export const createBugOnServerEvent = createEvent<{
  reportId: number;
  receive: string;
  expect: string;
}>();

// Событие для создания бага по расфокусу
export const createBugOnBlurEvent = createEvent<{
  clientId: number;
  field: ResultFieldTypes;
  value: string;
}>();
export const promoteLocalBugToBackendEvent = createEvent<{
  tempClientId: number;
  backendBug: CreateBugResponse & { reportId: number };
}>();

// cобытие для управления фокусом
export const setFocusedBugEvent = createEvent<number>();

/** Сторы */

// Стор для одного локального бага
export const $newLocalBugStore = createStore<LocalBug>({
  receive: "",
  expect: "",
  hasCreatedOnBackend: false,
  reportId: null,
  clientId: null,
})
  // инициализация
  .on(setLocalBugReportEvent, (state, { reportId, clientId }) => ({
    ...state,
    reportId,
    clientId,
    receive: "",
    expect: "",
    hasCreatedOnBackend: false,
  }))
  // редактируем поля по clientId
  .on(updateLocalBugFieldEvent, (state, { field, value }) => ({
    ...state,
    [field]: value,
  }))
  // при клике «добавить баг» заводим новый clientId и фокусируем его
  .on(createLocalBugEvent, (state, { reportId }) => {
    const tempId = Date.now();
    return {
      receive: "",
      expect: "",
      hasCreatedOnBackend: false,
      reportId,
      clientId: tempId,
    };
  })
  // после отправки на сервер — ставим флаг
  .on(createBugOnServerEvent, (state) => ({
    ...state,
    hasCreatedOnBackend: true,
  }))
  .reset(clearLocalBugEvent);

// Стор для списка локальных багов
export const $localBugsStore = createStore<BugClientEntity[]>([])
  // создаём временный баг в списке
  .on(createLocalBugEvent, (state, { reportId }) => {
    const now = new Date().toISOString();
    const tempId = Date.now();
    const newLocal: BugClientEntity = {
      id: tempId,
      clientId: tempId,
      receive: "",
      expect: "",
      status: BugStatuses.ACTIVE,
      createdAt: now,
      updatedAt: now,
      creatorUserId: "",
      reportId,
      attachments: null,
      comments: null,
      isLocalOnly: true,
    };
    return [...state, newLocal];
  })
  // «прокачиваем» локальный баг данными с бэка, но НЕ меняем clientId
  .on(promoteLocalBugToBackendEvent, (state, { tempClientId, backendBug }) => {
    return state.map((bug) =>
      bug.clientId === tempClientId
        ? {
            ...bug,
            id: backendBug.id,
            isLocalOnly: false,
            creatorUserId: backendBug.creatorUserId,
            createdAt: backendBug.createdAt,
            updatedAt: backendBug.updatedAt,
          }
        : bug
    );
  })
  // удаляем локальный баг только по явному removeLocalBugEvent
  .on(removeLocalBugEvent, (state, { clientId }) =>
    state.filter((b) => b.clientId !== clientId)
  )
  .reset(clearLocalBugEvent);

export const $focusedBugClientId = createStore<number | null>(null).on(
  setFocusedBugEvent,
  (_, clientId) => clientId
);

/** Сэмплы */

sample({
  source: $user,
  clock: createBugOnServerEvent,
  fn: (user, { reportId, receive, expect }) => {
    const data: Partial<BugFormData> = { status: BugStatuses.ACTIVE };
    if (receive.trim()) data.receive = receive.trim();
    if (expect.trim()) data.expect = expect.trim();
    return { reportId, data: { ...data, creatorUserId: user.id } };
  },
  target: createBugFx,
});

sample({
  clock: createBugFx.doneData,
  source: $newLocalBugStore,
  filter: (state, newBug) => newBug.reportId === state.reportId,
  fn: (state, newBug) => ({
    tempClientId: state.clientId!,
    backendBug: newBug,
  }),
  target: promoteLocalBugToBackendEvent,
});

sample({
  clock: createLocalBugEvent,
  source: $newLocalBugStore,
  filter: (state) => state.clientId != null,
  fn: (state) => state.clientId!,
  target: setFocusedBugEvent,
});

// Подготовка данных для создания бага по расфокусу
const $readyToCreateBugOnBlur = sample({
  clock: createBugOnBlurEvent,
  source: $newLocalBugStore,
  filter: (state, { clientId }) =>
    state.clientId === clientId &&
    !state.hasCreatedOnBackend &&
    state.reportId !== null,
  fn: (state, { field, value }) => {
    const newReceive = field === BugResultTypes.RECEIVE ? value : state.receive;
    const newExpect = field === BugResultTypes.EXPECT ? value : state.expect;

    // Создаем баг только если есть хотя бы одно заполненное поле
    if (newReceive.trim() || newExpect.trim()) {
      return {
        reportId: state.reportId!,
        receive: newReceive,
        expect: newExpect,
      };
    }
    return null;
  },
});

// Создание бага по расфокусу
sample({
  clock: $readyToCreateBugOnBlur,
  filter: (payload): payload is NonNullable<typeof payload> => payload !== null,
  target: createBugOnServerEvent,
});

sample({
  clock: promoteLocalBugToBackendEvent,
  fn: ({ tempClientId }) => tempClientId,
  target: setFocusedBugEvent,
});
