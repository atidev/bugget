import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
} from "effector";
import { fetchReport, createReport, updateReport } from "@/api/reports";
import { ReportStatuses, RequestStates } from "@/const";
import { User } from "@/types/user";
import { NewReport, ReportFormUIData, Report } from "@/types/report";
import { ReportResponse } from "@/api/reports/models";

const defaultFormData = {
  id: null,
  title: "",
  status: ReportStatuses.IN_PROGRESS,
  responsible: null,
  participants: [],
  responsibleId: "",
  creator: null,
  createdAt: null,
  updatedAt: null,
  bugs: [],
};

const convertBackResponseToStoreModel = (reportResponse: ReportResponse) => ({
  id: reportResponse.id,
  title: reportResponse.title || "",
  status: reportResponse.status,
  responsible: reportResponse.responsible,
  creator: reportResponse.creator,
  createdAt: new Date(reportResponse.createdAt),
  updatedAt: new Date(reportResponse.updatedAt),
  participants: reportResponse.participants,
  bugs:
    reportResponse.bugs?.map((bug) => {
      return {
        ...bug,
        createdAt: new Date(bug.createdAt),
        updatedAt: new Date(bug.updatedAt),
        comments: bug.comments.map((comment) => ({
          ...comment,
          createdAt: new Date(comment.createdAt),
          updatedAt: new Date(comment.updatedAt),
        })),
        isChanged: false,
      };
    }) || [],
  responsibleId: reportResponse.responsible.id,
});

export const fetchReportFx = createEffect(async (id: number) => {
  const data = await fetchReport(id);
  return data;
});

export const createReportFx = createEffect(async (newReport: NewReport) => {
  const data = await createReport(newReport);
  return data;
});

export const updateReportFx = createEffect(
  async (report: {
    id: number | null;
    title: string | null;
    status: ReportStatuses | null;
    responsibleId: string | null;
  }) => {
    if (!report.id) return;
    const payload = {
      title: report.title,
      status: report.status,
      responsible: report.responsibleId,
    };
    return await updateReport(payload, report.id);
  }
);

export const updateReportEvent = createEvent();
export const clearReport = createEvent();
export const resetReport = createEvent();

export const updateTitle = createEvent<string>();
export const updateStatus = createEvent<number>();
export const updateResponsible = createEvent<User | null>();

export const setIsNewReport = createEvent();

export const $isNewReport = createStore(true)
  .on(setIsNewReport, (_, isNew) => isNew)
  .reset(clearReport);

export const $reportRequestState = createStore(RequestStates.IDLE);
$reportRequestState
  .on(fetchReportFx.pending, (_, state) => {
    return state ? RequestStates.PENDING : RequestStates.DONE;
  })
  .on(fetchReportFx.doneData, () => {
    return RequestStates.DONE;
  });

export const $isReportChanged = createStore(false).reset(clearReport);

export const $initialReportForm = createStore<Report>(defaultFormData)
  .on(fetchReportFx.doneData, (_, report) =>
    convertBackResponseToStoreModel(report)
  )
  .reset(clearReport);

// Текущее состояние репорта (изменяемые данные)
export const $reportForm = createStore<ReportFormUIData>(defaultFormData)
  .on(fetchReportFx.doneData, (_, report) => {
    return {
      id: report.id,
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

// сбрасываем report при сбросе
sample({
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
  source: $combinedForm, // { reportForm, initialForm }
  clock: updateReportEvent,
  fn: ({ reportForm, initialForm }) => {
    const responsibleId = reportForm.responsible?.id || null;
    const initialResponsibleId = initialForm.responsible?.id || null;

    const diff = {
      id: reportForm.id,
      title: reportForm.title !== initialForm.title ? reportForm.title : null,
      status:
        reportForm.status !== initialForm.status ? reportForm.status : null,
      responsibleId:
        responsibleId &&
        initialResponsibleId &&
        initialResponsibleId !== responsibleId
          ? responsibleId
          : null,
    };

    return diff;
  },
  target: updateReportFx,
});

sample({
  source: updateReportFx.doneData, // После успешного обновления отчета
  target: [$reportForm, $initialReportForm], // Обновляем оба стора
});
