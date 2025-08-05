import { useEffect } from "react";

import { useUnit, useStoreMap } from "effector-react";

import { AttachmentTypes, BugResultTypes, BugStatuses } from "@/const";
import { useSocketEvent } from "@/hooks/useSocketEvent";
import {
  uploadAttachmentEvent,
  deleteAttachmentEvent,
  $attachmentsData,
} from "@/store/attachments";
import { patchBugSocketEvent, updateBugDataEvent } from "@/store/bugs";
import {
  $focusedBugClientId,
  $newLocalBugStore,
  setLocalBugReportEvent,
  updateLocalBugFieldEvent,
  createBugOnBlurEvent,
} from "@/store/localBugs";
import { $reportIdStore } from "@/store/report";
import { BugClientEntity, BugFormData, ResultFieldTypes } from "@/types/bug";
import { SocketEvent } from "@/webSocketApi/models";

import BugHeader from "./components/BugHeader/BugHeader";
import Result from "./components/Result/Result";

type Props = {
  bug: BugClientEntity;
};

const Bug = ({ bug }: Props) => {
  const reportId = useUnit($reportIdStore);
  const newBug = useUnit($newLocalBugStore);
  const focusedClientId = useUnit($focusedBugClientId);

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

  // Инициализация стора для нового бага
  useEffect(() => {
    if (bug.isLocalOnly && reportId) {
      setLocalBugReportEvent({ reportId, clientId: bug.clientId });
    }
  }, [bug.clientId, bug.isLocalOnly, reportId]);

  useSocketEvent(SocketEvent.BugPatch, (patch) => {
    patchBugSocketEvent({ bugId: patch.bugId, patch: patch.patch });
  });

  const updateBugFields = (bugId: number, data: Partial<BugFormData>) => {
    if (!reportId) return;
    updateBugDataEvent({ bugId, reportId, data });
  };

  const handleTemporaryBugChange = (field: ResultFieldTypes, value: string) => {
    updateLocalBugFieldEvent({ clientId: bug.clientId, field, value });
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

  const handleReceiveBlur = (value: string) => {
    if (bug.isLocalOnly) {
      createBugOnBlurEvent({
        clientId: bug.clientId,
        field: BugResultTypes.RECEIVE,
        value,
      });
    }
  };

  const handleExpectBlur = (value: string) => {
    if (bug.isLocalOnly) {
      createBugOnBlurEvent({
        clientId: bug.clientId,
        field: BugResultTypes.EXPECT,
        value,
      });
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
      <BugHeader bug={bug} onStatusChange={handleStatusChange} />

      <Result
        title="фактический результат"
        value={bug.isLocalOnly ? newBug.receive : bug.receive || ""}
        onSave={handleReceiveChange}
        onBlur={handleReceiveBlur}
        colorType="error"
        autoFocus={bug.clientId === focusedClientId}
        attachments={receiveAttachments}
        reportId={reportId}
        bugId={bug.id}
        attachType={AttachmentTypes.FACT}
        onAttachmentUpload={() => handleAttachmentUpload(AttachmentTypes.FACT)}
        onAttachmentDelete={handleDeleteAttachment}
      />

      <Result
        title="ожидаемый результат"
        value={bug.isLocalOnly ? newBug.expect : bug.expect || ""}
        onSave={handleExpectChange}
        onBlur={handleExpectBlur}
        colorType="success"
        autoFocus={false}
        attachments={expectAttachments}
        reportId={reportId}
        bugId={bug.id}
        attachType={AttachmentTypes.EXPECT}
        onAttachmentUpload={() =>
          handleAttachmentUpload(AttachmentTypes.EXPECT)
        }
        onAttachmentDelete={handleDeleteAttachment}
      />
    </div>
  );
};

export default Bug;
