import { useUnit, useStoreMap } from "effector-react";
import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import { CircleSmall } from "lucide-react";
import { useState, useEffect } from "react";

import { AttachmentTypes, BugResultTypes, BugStatuses } from "@/const";
import { BugEntity, BugFormData, ResultFieldTypes } from "@/types/bug";
import {
  patchBugSocketEvent,
  updateBugDataEvent,
  createBugEvent,
  createBugFx,
} from "@/store/bugs";
import {
  uploadAttachmentEvent,
  deleteAttachmentEvent,
} from "@/store/attachments";
import { $reportIdStore } from "@/store/report";
import { $attachmentsData } from "@/store/attachments";
import { SocketEvent } from "@/webSocketApi/models";
import { useSocketEvent } from "@/hooks/useSocketEvent";

import BugStatusSelect from "./components/BugStatusSelect/BugStatusSelect";

import Result from "./components/Result/Result";
import { CreateBugResponse } from "@/api/bugs/models";

type Props = {
  bug: BugEntity;
  onRemoveTemporary?: (bugId: number) => void;
};

const Bug = ({ bug, onRemoveTemporary }: Props) => {
  const reportId = useUnit($reportIdStore);

  const allAttachments = useStoreMap({
    store: $attachmentsData,
    keys: [bug.id],
    fn: ({ attachments, bugAttachments }, [bugId]) => {
      if (!bugId) return [];
      const attachmentIds = bugAttachments[bugId] || [];
      return attachmentIds.map((id) => attachments[id]).filter(Boolean);
    },
  });

  const receiveAttachments = allAttachments.filter(
    (att) => att.attachType === AttachmentTypes.FACT
  );
  const expectAttachments = allAttachments.filter(
    (att) => att.attachType === AttachmentTypes.EXPECT
  );

  // Локальное состояние для текущего нового бага
  const [localReceive, setLocalReceive] = useState("");
  const [localExpect, setLocalExpect] = useState("");
  const [hasCreatedRealBug, setHasCreatedRealBug] = useState(false);

  // Сброс локального состояния при смене бага
  useEffect(() => {
    setLocalReceive("");
    setLocalExpect("");
    setHasCreatedRealBug(false);
  }, [bug.id]);

  // Подписка на успешное создание бага на бэке для удаления локального фронтового
  useEffect(() => {
    if (!onRemoveTemporary) return;

    const subscription = createBugFx.doneData.watch(
      (newBug: CreateBugResponse & { reportId: number }) => {
        // Если создался баг на бэкенде вместо текущего фронтового - удаляем текущий фронтовый баг
        if (newBug.reportId === bug.reportId) {
          onRemoveTemporary(bug.id);
        }
      }
    );

    return () => subscription.unsubscribe();
  }, [bug.reportId, bug.id, onRemoveTemporary]);

  useSocketEvent(SocketEvent.BugPatch, (patch) => {
    patchBugSocketEvent({ bugId: patch.bugId, patch: patch.patch });
  });

  const updateBugFields = (bugId: number, data: Partial<BugFormData>) => {
    if (!reportId) return;
    updateBugDataEvent({ bugId, reportId, data });
  };

  const handleTemporaryBugChange = (field: ResultFieldTypes, value: string) => {
    if (hasCreatedRealBug) return;

    const newReceive = field === BugResultTypes.RECEIVE ? value : localReceive;
    const newExpect = field === BugResultTypes.EXPECT ? value : localExpect;

    if (field === BugResultTypes.RECEIVE) {
      setLocalReceive(value);
    } else {
      setLocalExpect(value);
    }

    if (newReceive.trim() || newExpect.trim()) {
      setHasCreatedRealBug(true);
      const data: Partial<BugFormData> = { status: BugStatuses.ACTIVE };
      if (newReceive.trim()) data.receive = newReceive.trim();
      if (newExpect.trim()) data.expect = newExpect.trim();

      createBugEvent({
        reportId: reportId!,
        data,
      });
    }
  };

  const handleExistingBugChange = (field: ResultFieldTypes, value: string) => {
    const data: Partial<BugFormData> = {};
    if (value.trim()) {
      data[field] = value.trim();
    }
    updateBugFields(bug.id, data);
  };

  const handleReceiveChange = (newReceive: string) => {
    if (bug.isLocalOnly) {
      handleTemporaryBugChange(BugResultTypes.RECEIVE, newReceive);
    } else {
      handleExistingBugChange(BugResultTypes.RECEIVE, newReceive);
    }
  };

  const handleExpectChange = (newExpect: string) => {
    if (bug.isLocalOnly) {
      handleTemporaryBugChange(BugResultTypes.EXPECT, newExpect);
    } else {
      handleExistingBugChange(BugResultTypes.EXPECT, newExpect);
    }
  };

  const handleStatusChange = (status: BugStatuses) => {
    updateBugFields(bug.id, { status });
  };

  const handleAttachmentUpload =
    (attachType: AttachmentTypes) => (file: File) => {
      if (!reportId || bug.isLocalOnly) return;

      uploadAttachmentEvent({
        reportId,
        bugId: bug.id,
        attachType,
        file,
      });
    };

  const handleDeleteAttachment = (attachmentId: number) => {
    if (!reportId || bug.isLocalOnly) return;

    deleteAttachmentEvent({
      reportId,
      bugId: bug.id,
      attachmentId,
    });
  };

  return (
    <div
      className={`card bg-base-100 shadow-lg border border-base-300 mb-4 p-4 grid grid-cols-2 gap-4 ${
        bug.status === BugStatuses.ARCHIVED ? "border-success" : ""
      }`}
    >
      <div className="col-span-2 flex justify-between items-center mb-4">
        <div className="flex items-center gap-2">
          <span
            id={bug.id?.toString()}
            className="text-lg font-bold min-h-[28px] flex items-center"
          >
            {bug.isLocalOnly ? "Новый баг" : `Баг #${bug.id}`}
          </span>
          {!bug.isLocalOnly && bug.createdAt && (
            <span className="text-sm text-base-content/60">
              Создан{" "}
              {formatDistanceToNow(new Date(bug.createdAt), {
                addSuffix: true,
                locale: ru,
              })}
            </span>
          )}
        </div>
        {!bug.isLocalOnly && (
          <BugStatusSelect status={bug.status} onChange={handleStatusChange} />
        )}
      </div>

      <Result
        title={
          <span className="inline-flex items-center">
            <CircleSmall
              size={20}
              color="var(--color-error)"
              fill="var(--color-error)"
            />{" "}
            фактический результат
          </span>
        }
        value={bug.isLocalOnly ? localReceive : bug.receive || ""}
        onSave={handleReceiveChange}
        colorType="error"
        autoFocus={bug.isLocalOnly} // автофокус для нового бага на фронте
        attachments={receiveAttachments}
        reportId={reportId || undefined}
        bugId={bug.id || undefined}
        attachType={AttachmentTypes.FACT}
        onFileUpload={() => handleAttachmentUpload(AttachmentTypes.FACT)}
        onDeleteAttachment={handleDeleteAttachment}
      />

      <Result
        title={
          <span className="inline-flex items-center">
            <CircleSmall
              size={20}
              color="var(--color-success)"
              fill="var(--color-success)"
            />{" "}
            ожидаемый результат
          </span>
        }
        value={bug.isLocalOnly ? localExpect : bug.expect || ""}
        onSave={handleExpectChange}
        colorType="success"
        attachments={expectAttachments}
        reportId={reportId}
        bugId={bug.id}
        attachType={AttachmentTypes.EXPECT}
        onFileUpload={() => handleAttachmentUpload(AttachmentTypes.EXPECT)}
        onDeleteAttachment={handleDeleteAttachment}
      />
    </div>
  );
};

export default Bug;
