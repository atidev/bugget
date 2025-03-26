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
import Dropdown from "@/components/Dropdown/Dropdown";

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
    <div className={`report-form p-4 mb-3 bg-base-100 rounded-box shadow-lg border border-gray-300 ${reportForm.status === Number(ReportStatuses.READY)
      ? "border-success"
      : ""
      }`}>
      <div className="flex items-center justify-between items-start">
        {isNewReport ? (
          <span className="text-2xl">Новый репорт</span>
        ) : (
          <span className="text-2xl">
            Репорт<span className="text-gray-300">#{reportForm.id}</span>
          </span>
        )}

        {!isNewReport && (
          <Dropdown
            className="max-w-[150px]"
            value={reportForm.status}
            onChange={(v) => {
              setUpdateStatus(Number(v) as ReportStatuses)
            }}
            options={[
              { label: 'В работе', value: ReportStatuses.READY },
              { label: 'Решён', value: ReportStatuses.IN_PROGRESS },
            ]}
          />
        )}
      </div>
      <div>
        <div className="text-xs font-semibold mt-1 mb-1">Заголовок</div>
        <input
          type="text"
          value={reportForm.title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Описание проблемы в двух словах"
          className="input input-bordered w-full focus:outline-none"
          maxLength={255}
        />
      </div>

      <div className="flex justify-between items-end">
        <div>
          <div className="text-xs font-semibold mb-1">Ответственный</div>

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
