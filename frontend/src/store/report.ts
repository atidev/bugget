import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
} from "effector";
import { fetchReport, createReport, updateReport } from "../api/report";
import { Report } from "@/types/report";
import { User } from "@/types/user";
import { ReportStore } from "@/types/stores";

export const fetchReportFx = createEffect(async (id: number) => {
  const data = await fetchReport(id);
  return data;
});

export const createReportFx = createEffect(async (newReport: ReportStore) => {
  const data = await createReport(newReport);
  return data;
});

export const updateReportFx = createEffect(async (request: Report) => {
  const payload = {
    title: request.title,
    status: request.status,
    responsibleUserId: request.responsible?.id,
  };
  return await updateReport(payload, request?.id);
});

export const updateReportEvent = createEvent();
export const clearReport = createEvent();
export const resetReport = createEvent();

export const updateTitle = createEvent<string>();
export const updateStatus = createEvent<number>();
export const updateResponsible = createEvent<User | null>();

export const setIsNewReport = createEvent<boolean>();

export const $isNewReport = createStore<boolean>(true)
  .on(setIsNewReport, (_, isNew) => isNew)
  .reset(clearReport);

export const $isReportChanged = createStore<boolean>(false).reset(clearReport);

export const $initialReportForm = createStore<Report | null>(null)
  .on(fetchReportFx.doneData, (_, report) => report)
  .reset(clearReport);

// Текущее состояние репорта (изменяемые данные)
export const $reportForm = createStore<{
  id: number | null;
  title: string;
  status: number;
  responsible: User | null;
  participants: User[];
}>({
  id: null,
  title: "",
  status: 0,
  responsible: null,
  participants: [],
})
  .on(fetchReportFx.doneData, (_, report) => {
    return {
      id: report.id || null,
      title: report.title || "",
      status: report.status || 0,
      responsible: report.responsible || {},
      participants: report.participants || [],
    };
  })
  .on(updateTitle, (state, title) => ({ ...state, title }))
  .on(updateResponsible, (state, responsible) => ({ ...state, responsible }))
  .on(updateStatus, (state, status) => ({ ...state, status }))
  .reset(clearReport);

const $combinedForm = combine({
  reportForm: $reportForm,
  initialForm: $initialReportForm,
});

// Проверяем изменения, когда $reportForm или $initialReportForm обновляются
sample({
  source: { form: $reportForm, initial: $initialReportForm },
  fn: ({ form, initial }) => {
    if (!initial) return false; // Если начальные данные отсутствуют, считаем, что изменений нет
    return (
      form.title !== initial.title ||
      form.status !== initial.status ||
      form.responsible?.id !== initial.responsible?.id
    );
  },
  target: $isReportChanged, // Записываем в стор
});

// сбарасываем report при сбросе
sample({
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  // todo fix ts-ignore
  source: $initialReportForm, // Берём данные из исходного стейта (загруженного с сервера)
  clock: resetReport, // Ждём, когда сработает resetReport
  target: $reportForm, // Копируем данные в редактируемый стор
});

// При загрузке отчёта устанавливаем `isNewReport`
sample({
  source: fetchReportFx.doneData,
  fn: () => false, // Если отчёт загружен, это НЕ новый отчёт
  target: setIsNewReport,
});

// При создании отчёта обновляем `isNewReport`
sample({
  source: createReportFx.doneData,
  fn: () => false, // После успешного создания отчёта считаем его существующим
  target: setIsNewReport,
});

sample({
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  // todo fix ts-ignore
  source: $combinedForm, // Здесь объект { reportForm, initialForm }
  clock: updateReportEvent,
  fn: ({ reportForm, initialForm }) => {
    // Используем правильные имена
    if (!initialForm) return null;

    const updatedReport: Partial<typeof reportForm> = {
      id: reportForm.id,
      title:
        reportForm.title !== initialForm.title ? reportForm.title : undefined,
      status:
        reportForm.status !== initialForm.status
          ? reportForm.status
          : undefined,
      responsible:
        reportForm.responsible?.id !== initialForm.responsible?.id
          ? reportForm.responsible
          : undefined,
    };

    return Object.values(updatedReport).some((value) => value !== undefined)
      ? updatedReport
      : null;
  },
  target: updateReportFx,
});

sample({
  source: updateReportFx.doneData, // После успешного обновления отчета
  target: [$reportForm, $initialReportForm], // Обновляем оба стора
});
