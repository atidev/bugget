import { useUnit } from "effector-react";
import {
  $reportForm,
  updateTitle,
  updateResponsible,
  resetReport,
  $isReportChanged,
  $isNewReport,
  $reportRequestState,
  updateStatus,
  updateReportEvent,
} from "@/store/report";
import "./ReportHeader.css";
import CancelButton from "@/components/CancelButton/CancelButton";
import SaveButton from "@/components/SaveButton/SaveButton";
import Autosuggest from "@/components/Autosuggest/Autosuggest";
import Avatar from "@/components/Avatar/Avatar";
import { ReportStatuses, RequestStates } from "@/const";
import Dropdown from "@/components/Dropdown/Dropdown";
import { employeesAutocomplete } from "@/api/employees";
import { User } from "@/types/user";
import ParticipantsSkeleton from "./components/ParticipantsSkeleton";

const autocompleteUsers = async (searchString: string) => {
  const response = await employeesAutocomplete(searchString);
  return (response.employees ?? []).map((employee: User) => ({
    id: employee.id,
    display: employee.name,
  }));
};

const ReportHeader = () => {
  const [
    reportForm,
    setTitle,
    setResponsibleId,
    isReportChanged,
    reportRequestState,
    reset,
    isNewReport,
    setUpdateStatus,
    updateReport,
  ] = useUnit([
    $reportForm,
    updateTitle,
    updateResponsible,
    $isReportChanged,
    $reportRequestState,
    resetReport,
    $isNewReport,
    updateStatus,
    updateReportEvent,
  ]);

  const handleUserSelect = (user: User | null) => {
    setResponsibleId(user);
  };

  return (
    <div
      className={`report-header p-4 mb-3 card card-border shadow-lg border-gray-300 gap-2 ${
        reportForm.status === Number(ReportStatuses.READY) &&
        reportRequestState === RequestStates.DONE
          ? "border-success"
          : ""
      }`}
    >
      <div className="flex justify-between items-center">
        {reportRequestState !== RequestStates.DONE ? (
          <div className="skeleton min-h-[40px]" />
        ) : (
          <span className="text-2xl">
            {!isNewReport ? (
              <>
                {" "}
                Репорт <span className="text-gray-300">#{reportForm.id} </span>
              </>
            ) : (
              <span>Новый репорт</span>
            )}
          </span>
        )}

        {!isNewReport && (
          <Dropdown
            className="max-w-[150px]"
            value={reportForm.status}
            onChange={(selected) => {
              setUpdateStatus(Number(selected));
            }}
            options={[
              { label: "Решён", value: ReportStatuses.READY },
              { label: "В работе", value: ReportStatuses.IN_PROGRESS },
            ]}
          />
        )}
      </div>
      <div>
        <div className="text-xs font-semibold mt-1 mb-1">Заголовок</div>
        {reportRequestState !== RequestStates.DONE ? (
          <div className="skeleton input w-full" />
        ) : (
          <input
            type="text"
            value={reportForm.title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Описание проблемы в двух словах"
            className="input input-bordered w-full focus:outline-none"
            maxLength={255}
          />
        )}
      </div>

      <div className="flex justify-between items-end">
        <div>
          <div className="text-xs font-semibold mb-1">Ответственный</div>
          <div className="flex gap-4 items-center">
            {reportRequestState !== RequestStates.DONE ? (
              <div className="skeleton input shrink-0 min-w-[288px]" />
            ) : (
              <Autosuggest
                onSelect={(entity) =>
                  handleUserSelect(
                    entity ? { id: entity.id, name: entity.display } : null
                  )
                }
                externalString={reportForm.responsible?.name || ""}
                autocompleteFn={autocompleteUsers}
              />
            )}
            <div className="participants-wrapper">
              {reportRequestState !== RequestStates.DONE ? (
                <ParticipantsSkeleton />
              ) : (
                !!reportForm.participants?.length &&
                reportForm.participants.map((p) => (
                  <div className="tooltip" key={p.id}>
                    <Avatar />
                    <span className="tooltiptext rounded">{p.name}</span>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>
        {!isNewReport && isReportChanged && reportForm.responsible?.id && (
          <div className="flex gap-2 justify-end">
            <CancelButton isChanged={isReportChanged} onReset={reset} />
            <SaveButton isChanged={isReportChanged} onSave={updateReport} />
          </div>
        )}
      </div>
    </div>
  );
};

export default ReportHeader;
