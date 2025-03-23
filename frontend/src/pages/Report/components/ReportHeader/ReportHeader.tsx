import { useUnit } from "effector-react";
import {
  $reportForm,
  updateTitle,
  updateResponsible,
  resetReport,
  $isReportChanged,
  $isNewReport,
  updateStatus,
  updateReportEvent,
} from "@/store/report";
import "./ReportHeader.css";
import CancelButton from "@/components/CancelButton/CancelButton";
import SaveButton from "@/components/SaveButton/SaveButton";
import UsersAutosuggest from "../UsersAutosuggest/UsersAutosuggest";
import Avatar from "@/components/Avatar/Avatar";
import { ReportStatuses } from "../../../../const";

const ReportHeader = () => {
  const [
    reportForm,
    setTitle,
    setResponsibleId,
    isReportChanged,
    reset,
    isNewReport,
    setUpdateStatus,
    updateReport,
  ] = useUnit([
    $reportForm,
    updateTitle,
    updateResponsible,
    $isReportChanged,
    resetReport,
    $isNewReport,
    updateStatus,
    updateReportEvent,
  ]);

  const handleUserSelect = (id: string, name: string) => {
    setResponsibleId({
      id,
      name,
    });
  };

  return (
    <div className="report-form p-4 mb-3 bg-base-100 rounded-box shadow-lg border border-gray-300">
      <div className="flex items-center justify-between">
        {isNewReport ? (
          <span className="text-2xl">Новый репорт</span>
        ) : (
          <span className="text-2xl">
            Репорт<span className="text-gray-300">#{reportForm.id}</span>
          </span>
        )}

        {!isNewReport && (
          <div className="select-wrapper">
            <select
              id="status"
              className={`select text-sm rounded-lg block w-full p-2.5 dark:placeholder-gray-400 dark:text-white`}
              // todo: remove double type convertion
              onChange={(event: React.ChangeEvent<HTMLSelectElement>) =>
                setUpdateStatus(Number(event.target.value) as ReportStatuses)
              }
              value={reportForm.status}
            >
              <option value={ReportStatuses.READY}>Решён</option>
              <option value={ReportStatuses.IN_PROGRESS}>Активен</option>
            </select>
          </div>
        )}
      </div>
      <div className="flex flex-col justify-between items-start mt-2">

        <label htmlFor="report-title" className="label mt-2 mb-1">
          <span className="label-text">Описание проблемы</span>
        </label>
        <textarea
          value={reportForm.title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Описание проблемы в двух словах"
          className="issue-description p-2 rounded-sm textarea textarea-bordered bg-base-200 resize-none focus:ring-1 focus:ring-gray-300 focus:border-gray-400"
          maxLength={255}
          rows={2}
          style={{
            height: "auto", // Автоматическая высота
            minHeight: "3rem", // Минимальная высота, чтобы было 2 строки
          }}
        />
      </div>

      <div className="flex justify-between items-end mt-2">
        <div>
          <label htmlFor="responsible" className="label mb-1">
            <span className="label-text">Ответственный</span>
          </label>
          <div className="flex gap-4 items-center">
            <UsersAutosuggest
              onSelect={handleUserSelect}
              externalString={reportForm.responsible?.name}
            />
            <div className="participants-wrapper">
              {reportForm.participants?.length > 0
                ? reportForm.participants.map((p) => (
                  <div className="tooltip" key={p.id}>
                    <Avatar />
                    <span key={p.id} className="tooltiptext rounded">
                      {p.name}
                    </span>
                  </div>
                ))
                : null}
            </div>
          </div>
        </div>
        {!isNewReport && isReportChanged && (
          <div className="flex gap-2 justify-end">
            <CancelButton isChanged={isReportChanged} onReset={reset} />
            <SaveButton
              isChanged={isReportChanged}
              onSave={() => updateReport()}
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default ReportHeader;
