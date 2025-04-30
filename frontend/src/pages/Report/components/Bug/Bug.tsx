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
  $bugRequestState,
} from "@/store/bugs";
import { $reportRequestState } from "@/store/report";
import { $attachmentsByBugId } from "@/store/attachments";
import "./Bug.css";
import CancelButton from "@/components/CancelButton/CancelButton";
import SaveButton from "@/components/SaveButton/SaveButton";
import Dropdown from "@/components/Dropdown/Dropdown";
import { AttachmentTypes, BugStatuses, RequestStates } from "@/const";
import Chat from "./components/Chat/Chat";
import { uploadAttachmentFx } from "@/store/attachments";
import { Bug as BugType } from "@/types/bug";
import Result from "./components/Result/Result";
import { ChangeEvent, useEffect, useRef } from "react";
import Heading from "./components/Heading/Heading";

type BugProps = {
  reportId?: number | null;
  isNewReport?: boolean;
  bugId?: number;
};

const Bug = ({ reportId, isNewReport, bugId }: BugProps) => {
  const [
    updateBugData,
    reset,
    updateBugApi,
    createBugApi,
    uploadAttachment,
    bugRequestState,
  ] = useUnit([
    updateBugEvent,
    resetBug,
    updateBugApiEvent,
    createBugEventByApi,
    uploadAttachmentFx,
    $bugRequestState,
  ]);

  const [newBugData, updateNewBugData] = useUnit([$newBugStore, updateNewBug]);
  const [reportRequestState] = useUnit([$reportRequestState]);
  const receivedTextareaRef = useRef<HTMLTextAreaElement>(null);
  const expectedTextareaRef = useRef<HTMLTextAreaElement>(null);

  const bug = useStoreMap({
    store: $bugsByBugId,
    keys: [bugId],
    fn: (state, [id]) => {
      return state && id
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

  useEffect(() => {
    const receivedTextarea = receivedTextareaRef.current;
    const expectedTextarea = expectedTextareaRef.current;
    if (!receivedTextarea || !expectedTextarea) return;

    receivedTextarea.style.height = "auto";
    expectedTextarea.style.height = "auto";

    const maxScrollHeight = Math.max(
      receivedTextarea.scrollHeight,
      expectedTextarea.scrollHeight
    );

    // 5пх -- число, чтобы высота была чуть выше, чтобы не показывался скролл
    const height = `${maxScrollHeight + 5}px`;
    receivedTextarea.style.height = height;
    expectedTextarea.style.height = height;
  }, [bug.receive, bug.expect]);

  const attachments = useStoreMap({
    store: $attachmentsByBugId,
    keys: [bugId],
    fn: (state, [id]) => {
      return id ? state[id] : [];
    },
  });

  const isNewBug = !bug.id;

  const isBugChanged = isNewBug
    ? newBugData.receive !== "" && newBugData.expect !== ""
    : bug.isChanged;

  // 3. Обработчик выбора файла
  const handleFileChange = async (
    event: React.ChangeEvent<HTMLInputElement>,
    attachmentType: number
  ) => {
    const file = event.target.files?.[0];
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
      event.target.value = "";
    }
  };

  // todo: move to derived stores
  const receivedFiles = attachments?.filter(
    (item) => item.attachType === AttachmentTypes.RECEIVED_RESULT
  );
  const expectedFiles = attachments?.filter(
    (item) => item.attachType === AttachmentTypes.EXPECTED_RESULT
  );

  const handleSave = () => {
    if (typeof bug.id === "number") {
      // todo убрать 'as', поправить типизацию
      updateBugApi(bug as BugType);
    } else if (bug.reportId) {
      createBugApi({ reportId: bug.reportId, bug: newBugData });
    }
  };

  const handleResultUpdate = (
    event: ChangeEvent<HTMLTextAreaElement>,
    key: string
  ) => {
    if (bug.id) {
      updateBugData({
        id: bug.id,
        [key]: event.target.value,
        status: bug.status,
      });
    } else {
      updateNewBugData({ [key]: event.target.value });
    }
  };

  return (
    <div
      className={`card card-border grid grid-cols-2 gap-2 p-4 mb-3 shadow-lg border-gray-300 ${
        bug.status === Number(BugStatuses.READY) ? "border-success" : ""
      }`}
    >
      <div className="flex items-center justify-between col-span-2">
        {reportRequestState !== RequestStates.DONE ? (
          <div className="skeleton min-h-[2em] min-w-[30%] shrink-0" />
        ) : (
          <Heading isNewBug={isNewBug} bugId={bug.id} />
        )}

        {/* Селект статуса (только для существующего бага) */}
        {!isNewBug && reportRequestState === RequestStates.DONE && (
          <Dropdown
            className="max-w-[150px]"
            onChange={(selected) => {
              if (bug.id) {
                updateBugData({ id: bug.id, status: Number(selected) });
              }
            }}
            value={bug.status}
            options={[
              { label: "Исправлен", value: BugStatuses.READY },
              { label: "Открыт", value: BugStatuses.IN_PROGRESS },
            ]}
          />
        )}
        {reportRequestState !== RequestStates.DONE && (
          <div className="skeleton min-h-[40px] min-w-[30%] shrink-0" />
        )}
      </div>
      <Result
        title="Фактический результат"
        value={isNewBug ? newBugData.receive : bug?.receive || ""}
        onChange={(event) => handleResultUpdate(event, "receive")}
        files={receivedFiles}
        onFileChange={(event) =>
          handleFileChange(event, AttachmentTypes.RECEIVED_RESULT)
        }
        withAttachments={!isNewBug}
        textareaRef={receivedTextareaRef}
      />
      <Result
        title="Ожидаемый результат"
        value={isNewBug ? newBugData.expect : bug?.expect || ""}
        onChange={(event) => handleResultUpdate(event, "expect")}
        files={expectedFiles}
        onFileChange={(event) =>
          handleFileChange(event, AttachmentTypes.EXPECTED_RESULT)
        }
        withAttachments={!isNewBug}
        textareaRef={expectedTextareaRef}
      />
      {/* Кнопки "Сохранить" / "Отмена" */}
      {!isNewReport && isBugChanged && (
        <div className="flex gap-2 justify-end mt-2 col-span-2">
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
          <SaveButton
            isChanged={isBugChanged}
            onSave={handleSave}
            isLoading={bugRequestState === RequestStates.PENDING}
          />
        </div>
      )}
      {!isNewBug && (
        <div className="mt-2 col-span-2">
          <Chat reportId={bug.reportId!} bugId={bug.id!} />
        </div>
      )}
    </div>
  );
};

export default Bug;
