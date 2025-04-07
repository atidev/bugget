import { useStoreMap, useUnit } from "effector-react";
import {
  $newBugStore,
  updateNewBug,
  createBugEventByApi,
} from "@/store/newBug";

import {
  $bugsByBugId,
  updateBugEvent,
  resetBug,
  updateBugApiEvent,
} from "@/store/bugs";
import { $attachmentsByBugId } from "@/store/attachments";
import "./Bug.css";
import CancelButton from "@/components/CancelButton/CancelButton";
import SaveButton from "@/components/SaveButton/SaveButton";
import Dropdown from "@/components/Dropdown/Dropdown";
import { BugStatuses } from "@/const";
import Chat from "./components/Chat/Chat";
import { uploadAttachmentFx } from "@/store/attachments";
import Result from "./components/Result/Result";
import { ChangeEvent } from "react";

type BugProps = {
  reportId?: number | null;
  bugId?: number;
  isLoading: boolean;
};

const Bug = ({ reportId, bugId, isLoading }: BugProps) => {
  const [updateBugData, reset, updateBugApi, createBugApi, uploadAttachment] =
    useUnit([
      updateBugEvent,
      resetBug,
      updateBugApiEvent,
      createBugEventByApi,
      uploadAttachmentFx,
    ]);

  const [newBugData, updateNewBugData] = useUnit([$newBugStore, updateNewBug]);

  const bug = useStoreMap({
    store: $bugsByBugId,
    keys: [bugId],
    fn: (state, [id]) => {
      return id
        ? state[id]
        : {
            id: bugId,
            status: Number(BugStatuses.IN_PROGRESS),
            reportId,
            receive: "",
            expect: "",
            isChanged: false,
            attachments: [],
            comments: [],
          };
    },
  });

  const attachments = useStoreMap({
    store: $attachmentsByBugId,
    keys: [bugId],
    fn: (state, [id]) => {
      return id ? state[id] : [];
    },
  });

  const isNewBug = bug.id === null || bug.id === undefined;

  const isBugChanged = isNewBug
    ? newBugData.receive !== "" && newBugData.expect !== ""
    : bug.isChanged;

  const isNewReport = reportId == null;

  // 3. Обработчик выбора файла
  const handleFileChange = async (
    e: React.ChangeEvent<HTMLInputElement>,
    attachmentType: number
  ) => {
    const file = e.target.files?.[0];
    if (!file || !bug.reportId || !bug.id) return;

    try {
      await uploadAttachment({
        reportId: bug.reportId,
        bugId: bug.id,
        file,
        attachType: attachmentType,
      });
    } catch (err) {
      console.error(err);
      alert("Ошибка при загрузке файла");
    } finally {
      // Сбросим value у input, чтобы повторно срабатывал onChange
      e.target.value = "";
    }
  };

  // todo: move to derived stores
  const receivedFiles = attachments?.filter((item) => item.attachType === 0);
  const expectedFiles = attachments?.filter((item) => item.attachType === 1);

  const handleSave = () => {
    if (!bug.reportId) return;
    if (isNewBug) {
      // Вызываем событие для создания нового бага
      createBugApi({ reportId: bug.reportId, bug: newBugData });
      return;
    }
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    // todo fix types
    updateBugApi(bug);
  };

  const handleResultUpdate = (
    event: ChangeEvent<HTMLTextAreaElement>,
    key: "receive" | "expect"
  ) => {
    if (isNewBug) {
      updateNewBugData({ [key]: event.target.value });
      return;
    }
    updateBugData({
      id: bug.id,
      receive: event.target.value,
      status: bug.status,
    });
  };

  return (
    <div
      className={`card card-border p-4 mb-3 shadow-lg border-gray-300 ${
        bug.status === Number(BugStatuses.READY) ? "border-success" : ""
      }`}
    >
      <div className="bug-content-wrapper">
        <div className="flex items-center justify-between">
          {isLoading ? (
            <div className="skeleton min-h-[2em] min-w-[30%] shrink-0" />
          ) : isNewBug ? (
            <span className="text-2xl"> Новый баг</span>
          ) : (
            <span className="text-2xl">
              Баг<span className="text-gray-300">#{bug.id}</span>
            </span>
          )}

          {/* Селект статуса (только для существующего бага) */}
          {!isNewBug && (
            <Dropdown
              className="max-w-[150px]"
              onChange={(selected) => {
                updateBugData({ id: bug.id!, status: Number(selected) });
              }}
              value={bug.status}
              options={[
                { label: "Исправлен", value: BugStatuses.READY },
                { label: "Открыт", value: BugStatuses.IN_PROGRESS },
              ]}
            />
          )}
        </div>
        <div className="flex grow-1 gap-3">
          <Result
            title="Фактический результат"
            value={isNewBug ? newBugData.receive : bug?.receive || ""}
            onChange={(event) => handleResultUpdate(event, "receive")}
            files={receivedFiles}
            onFileChange={(e) => handleFileChange(e, 0)}
            withAttachments={!isNewBug}
          />
          <Result
            title="Ожидаемый результат"
            value={isNewBug ? newBugData.expect : bug?.expect || ""}
            onChange={(event) => handleResultUpdate(event, "expect")}
            files={expectedFiles}
            onFileChange={(e) => handleFileChange(e, 0)}
            withAttachments={!isNewBug}
          />
        </div>
      </div>
      {/* Кнопки "Сохранить" / "Отмена" */}
      {!isNewReport && isBugChanged && (
        <div className="flex gap-2 justify-end mt-2">
          <CancelButton
            isChanged={isBugChanged}
            onReset={() => {
              if (isNewBug) {
                // Сброс нового бага
                updateNewBugData({ receive: "", expect: "" });
              } else {
                reset(bug.id!);
              }
            }}
          />
          <SaveButton isChanged={isBugChanged} onSave={handleSave} />
        </div>
      )}
      {!isNewBug && (
        <div className="mt-2">
          <Chat reportId={bug.reportId!} bugId={bug.id!} />
        </div>
      )}
    </div>
  );
};

export default Bug;
