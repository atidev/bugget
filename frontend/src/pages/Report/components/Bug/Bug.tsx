import React, { useEffect, useRef } from "react";
import { useStoreMap, useUnit } from "effector-react";
import {
  $newBugStore,
  updateNewBug,
  createBugEventByApi,
} from "../../../../store/newBug";

import {
  $bugsByBugId,
  updateBugEvent,
  resetBug,
  updateBugApiEvent,
} from "../../../../store/bugs";
import { $attachmentsByBugId } from "../../../../store/attachments";
import "./Bug.css";
import CancelButton from "../../../../components/CancelButton/CancelButton";
import SaveButton from "../../../../components/SaveButton/SaveButton";
import { BugStatuses } from "../../../../const";
import { Chat } from "./components/Chat/Chat";
import { uploadAttachmentFx } from "../../../../store/attachments";
import ImageCarousel from "./components/ImageCarousel/ImageCarousel";
import { BugStore } from "../../../../types/stores";

interface BugProps {
  reportId?: number | null;
  bugId?: number | null;
}

const Bug = ({ reportId, bugId }: BugProps) => {
  const textareaRefReceive = useRef<HTMLTextAreaElement>(null);
  const textareaRefExpect = useRef<HTMLTextAreaElement>(null);

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
        : ({
            id: bugId,
            status: Number(BugStatuses.IN_PROGRESS),
            reportId,
            receive: "",
            expect: "",
            isChanged: false,
          } as BugStore);
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
  // 2. Реф на file input, чтобы кликать по нему программно
  const fileInputRefRecieve = useRef<HTMLInputElement>(null);
  const fileInputRefExpected = useRef<HTMLInputElement>(null);

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

  const receivedFiles = attachments?.filter((item) => item.attachType === 0);
  const expectedFiles = attachments?.filter((item) => item.attachType === 1);

  useEffect(() => {
    if (textareaRefReceive.current) {
      textareaRefReceive.current.style.height = "auto";
      textareaRefReceive.current.style.height = `${textareaRefReceive.current.scrollHeight}px`;
    }
    if (textareaRefExpect.current) {
      textareaRefExpect.current.style.height = "auto";
      textareaRefExpect.current.style.height = `${textareaRefExpect.current.scrollHeight}px`;
    }
  }, [bug.receive, bug.expect]);

  return (
    <div
      className={`bug bg-base-200 rounded-box card-m shadow-xs border border-3 transition-color duration-200 ${
        bug.status === Number(BugStatuses.READY)
          ? "border-3 border-success shadow-lg"
          : "border-base-200"
      }`}
    >
      <div className="bug-content-wrapper">
        <div className="flex items-center justify-between">
          {isNewBug ? (
            <span className="text-2xl">Новый баг</span>
          ) : (
            <span className="text-2xl">
              Баг<span className="text-gray-300">#{bug.id}</span>
            </span>
          )}

          {/* Селект статуса (только для существующего бага) */}
          {!isNewBug && (
            <div className="select-wrapper">
              <select
                id="status"
                className={`select ${
                  bug.status === Number(BugStatuses.READY)
                    ? "select-success border-success"
                    : ""
                } text-sm rounded-lg block w-full p-2.5`}
                onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
                  const newStatus = event.target.value;
                  updateBugData({ id: bug.id!, status: Number(newStatus) });
                }}
                value={bug.status}
              >
                <option value={Number(BugStatuses.READY)}>Решён</option>
                <option value={Number(BugStatuses.IN_PROGRESS)}>Активен</option>
              </select>
            </div>
          )}
        </div>
        <div className="flex gap-3">
          <span className="w-1/2">Фактический результат</span>
          <span className="w-1/2">Ожидаемый результат</span>
        </div>
        <div className="bug-content">
          <textarea
            ref={textareaRefReceive}
            value={isNewBug ? newBugData.receive : bug?.receive}
            onChange={(e) =>
              isNewBug
                ? updateNewBugData({ receive: e.target.value })
                : updateBugData({
                    id: bug.id!,
                    receive: e.target.value,
                    status: bug.status,
                  })
            }
            className="textarea bug-section p-4 bg-base-100"
          />
          <textarea
            ref={textareaRefExpect}
            value={isNewBug ? newBugData.expect : bug?.expect}
            onChange={(e) =>
              isNewBug
                ? updateNewBugData({ expect: e.target.value })
                : updateBugData({
                    id: bug.id!,
                    expect: e.target.value,
                    status: bug.status,
                  })
            }
            className="textarea bug-section p-4 bg-base-100"
          />
        </div>
        <div className="flex gap-3">
          {/* Блок вложений */}
          {!isNewBug && (
            <div className="attachments w-1/2">
              {/* Скрытый input для выбора файла */}
              <input
                ref={fileInputRefRecieve}
                type="file"
                style={{ display: "none" }}
                onChange={(e) => handleFileChange(e, 0)}
              />

              {/* Кнопка "плюс" — открывает окно выбора файла */}
              {!isNewBug && (
                <button
                  className="btn btn-info btn-outline "
                  onClick={() => fileInputRefRecieve.current?.click()}
                >
                  + Добавить файл
                </button>
              )}
              {receivedFiles && !!receivedFiles.length && (
                <ImageCarousel attachments={receivedFiles} />
              )}
            </div>
          )}

          {!isNewBug && (
            <div className="attachments w-1/2">
              {/* Скрытый input для выбора файла */}
              <input
                ref={fileInputRefExpected}
                type="file"
                style={{ display: "none" }}
                onChange={(e) => handleFileChange(e, 1)}
              />

              {/* Кнопка "плюс" — открывает окно выбора файла */}
              {!isNewBug && (
                <button
                  className="btn btn-info btn-outline "
                  onClick={() => fileInputRefExpected.current?.click()}
                >
                  + Добавить файл
                </button>
              )}
              {expectedFiles && !!expectedFiles.length && (
                <ImageCarousel attachments={expectedFiles} />
              )}
            </div>
          )}
        </div>
        {/* Кнопки "Сохранить" / "Отмена" */}
        {!isNewReport && isBugChanged && (
          <div className="flex gap-2 justify-end">
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
              onSave={() => {
                if (isNewBug) {
                  // Вызываем событие для создания нового бага
                  createBugApi({ reportId: bug.reportId!, bug: newBugData });
                } else {
                  // Обновляем существующий баг
                  updateBugApi(bug);
                }
              }}
            />
          </div>
        )}

        {/* Комментарии (Chat) */}
        {!isNewBug && <Chat reportId={bug.reportId!} bugId={bug.id!} />}
      </div>
    </div>
  );
};

export default Bug;
