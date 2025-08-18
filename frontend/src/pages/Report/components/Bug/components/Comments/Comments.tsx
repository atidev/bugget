import { useState, useRef, useCallback } from "react";
import { Trash2, Send, MoreVertical, MessageCircle } from "lucide-react";
import { useStoreMap, useUnit } from "effector-react";
import {
  $commentsByBugId,
  createCommentFx,
  deleteCommentEvent,
  updateCommentEvent,
  createCommentAttachmentFx,
  deleteCommentAttachmentFx,
} from "@/store/comments";
import { $usersStore } from "@/store/report";
import { $user } from "@/store/user";
import Avatar from "@/components/Avatar/Avatar";
import FilePreview from "../FilePreview/FilePreview";
import AttachFileButton from "../AttachFileButton/AttachFileButton";

type Props = {
  reportId: number;
  bugId: number;
};

const Comments = ({ reportId, bugId }: Props) => {
  const [newCommentText, setNewCommentText] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingCommentId, setEditingCommentId] = useState<number | null>(null);
  const [editingText, setEditingText] = useState("");
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [commentAttachments, setCommentAttachments] = useState<File[]>([]);

  const comments = useStoreMap({
    store: $commentsByBugId,
    keys: [bugId],
    fn: (state, [id]) => state[id] || [],
  });

  const users = useUnit($usersStore);
  const currentUser = useUnit($user);
  const addCommentAttachment = useUnit(createCommentAttachmentFx);
  const removeCommentAttachment = useUnit(deleteCommentAttachmentFx);

  const handleSubmitComment = async () => {
    if (!newCommentText.trim() && commentAttachments.length === 0) return;

    setIsSubmitting(true);
    try {
      // Создаем комментарий через store
      const created = await createCommentFx({
        reportId,
        bugId,
        text: newCommentText.trim() || "Файл прикреплен",
      });

      // Если выбраны файлы, последовательно прикрепляем их к только что созданному комменту
      if (created?.id && commentAttachments.length > 0) {
        await Promise.all(
          commentAttachments.map((file) =>
            addCommentAttachment({
              reportId,
              bugId,
              commentId: created.id,
              file,
            })
          )
        );
      }

      // Очищаем форму
      setNewCommentText("");
      setCommentAttachments([]);
    } catch (error) {
      console.error("Ошибка при создании комментария:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files || []);
    setCommentAttachments((prev) => [...prev, ...files]);
    event.target.value = "";
  };

  const handleRemoveAttachment = (index: number) => {
    setCommentAttachments((prev) => prev.filter((_, i) => i !== index));
  };

  const handleDeleteComment = async (commentId: number) => {
    try {
      await deleteCommentEvent({ reportId, bugId, commentId });
    } catch (error) {
      console.error("Ошибка при удалении комментария:", error);
    }
  };

  const handleEditComment = (commentId: number, currentText: string) => {
    setEditingCommentId(commentId);
    setEditingText(currentText);
  };

  const handleCancelEdit = () => {
    setEditingCommentId(null);
    setEditingText("");
  };

  const handleSaveEdit = () => {
    if (!editingCommentId || !editingText.trim()) return;

    updateCommentEvent({
      reportId,
      bugId,
      commentId: editingCommentId,
      text: editingText.trim(),
    });
    setEditingCommentId(null);
    setEditingText("");
  };

  const handleDeleteCommentAttachment = (
    commentId: number,
    attachmentId: number
  ) => {
    removeCommentAttachment({ reportId, bugId, commentId, attachmentId });
  };

  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return "только что";

    return date.toLocaleTimeString("ru-RU", {
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return "только что";

    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    const diffInMinutes = diffInMs / (1000 * 60);
    const diffInHours = diffInMinutes / 60;
    const diffInDays = diffInHours / 24;

    // Если комментарий только что отправлен (менее 1 минуты)
    if (diffInMinutes < 1) {
      return "только что";
    }

    // Если комментарий отправлен сегодня (менее 24 часов)
    if (diffInDays < 1) {
      if (diffInHours < 1) {
        return `${Math.floor(diffInMinutes)}м назад`;
      }
      return `${Math.floor(diffInHours)}ч назад`;
    }

    // Если комментарий отправлен вчера
    if (diffInDays < 2) {
      return "вчера";
    }

    // Если комментарий отправлен более 2 дней назад
    return `${Math.floor(diffInDays)}д назад`;
  };

  const getCommentTimeDisplay = (dateString: string) => {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return "только что";

    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    const diffInDays = diffInMs / (1000 * 60 * 60 * 24);

    // Если комментарий отправлен сегодня (менее 24 часов)
    if (diffInDays < 1) {
      const diffInMinutes = diffInMs / (1000 * 60);

      // Если комментарий только что отправлен (менее 1 минуты)
      if (diffInMinutes < 1) {
        return "только что";
      }

      // Иначе показываем время
      return formatTime(dateString);
    }

    // Если комментарий отправлен вчера, показываем "вчера + время"
    if (diffInDays < 2) {
      return `вчера ${formatTime(dateString)}`;
    }

    // Если комментарий отправлен более 2 дней назад, показываем относительную дату
    return formatDate(dateString);
  };

  const getUserDisplayName = useCallback(
    (userId?: string, createdAt?: string) => {
      // 1) Точный матч: это текущий пользователь
      if (currentUser?.id && userId === currentUser.id) return "Вы";

      // 2) Пользователь найден в сторе по id
      if (userId && users[userId]?.name) return users[userId].name;

      // 3) Свежесозданный комментарий без userId: считаем, что это текущий пользователь
      if (currentUser?.id && (!userId || userId.trim() === "")) {
        if (createdAt) {
          const created = new Date(createdAt);
          if (
            !isNaN(created.getTime()) &&
            Date.now() - created.getTime() < 60 * 1000
          ) {
            return "Вы";
          }
        }
        // даже если времени нет, безопасный фолбек — "Вы", чтобы не было пустоты
        return "Вы";
      }

      // 4) Фолбек: показываем id, чтобы не было пустоты
      return userId || "Вы";
    },
    [users, currentUser?.id]
  );

  const handleKeyDownNewComment = (
    e: React.KeyboardEvent<HTMLTextAreaElement>
  ) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      if (newCommentText.trim() || commentAttachments.length > 0) {
        handleSubmitComment();
      }
    }
  };

  const handleKeyDownEditingComment = (
    e: React.KeyboardEvent<HTMLTextAreaElement>
  ) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      if (editingText.trim()) {
        handleSaveEdit();
      }
    }
  };

  return (
    <div className="bg-base-100 border border-base-300 rounded-lg p-2 col-span-2">
      {/* Плашечка с количеством комментариев */}
      {!!comments.length && (
        <div className="flex items-center gap-2 mb-2 p-2 bg-base-200 rounded-lg w-fit">
          <div className="w-5 h-5 rounded-full bg-info/20 flex items-center justify-center">
            <MessageCircle className="w-3 h-3 text-info" />
          </div>
          <span className="text-sm font-medium text-base-content">
            {comments.length}{" "}
            {comments.length === 1
              ? "комментарий"
              : comments.length < 5
              ? "комментария"
              : "комментариев"}
          </span>
        </div>
      )}
      {/* Список комментариев */}
      {!!comments.length && (
        <div className="space-y-2 mb-2">
          {comments.map((comment) => (
            <div key={comment.id} className="border-b border-base-300">
              <div className="flex items-start gap-3">
                <Avatar width={8} />

                <div className="flex-1 min-w-0 mb-2">
                  {/* Заголовок комментария */}
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <span className="text-xs font-medium text-base-content">
                        {getUserDisplayName(
                          comment.creatorUserId,
                          comment.createdAt
                        )}
                      </span>
                      <span className="text-xs text-base-content/60">
                        {getCommentTimeDisplay(comment.createdAt)}
                      </span>
                    </div>

                    {/* Меню опций */}
                    <div className="dropdown dropdown-end">
                      <button className="btn btn-ghost btn-xs p-1">
                        <MoreVertical className="w-4 h-4" />
                      </button>
                      <ul className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-32">
                        <li>
                          <button
                            className="text-info"
                            onClick={() =>
                              handleEditComment(comment.id, comment.text)
                            }
                          >
                            Редактировать
                          </button>
                        </li>
                        <li>
                          <button
                            className="text-error"
                            onClick={() => handleDeleteComment(comment.id)}
                          >
                            Удалить
                          </button>
                        </li>
                      </ul>
                    </div>
                  </div>

                  {/* Текст комментария */}
                  {editingCommentId === comment.id ? (
                    <div className="space-y-2">
                      <textarea
                        value={editingText}
                        onChange={(event) => setEditingText(event.target.value)}
                        onKeyDown={handleKeyDownEditingComment}
                        className="textarea textarea-bordered w-full resize-none min-h-auto"
                        placeholder="Введите текст комментария... (Enter для сохранения, Cmd+Option+Enter для новой строки)"
                        rows={1}
                      />
                      <div className="flex gap-2">
                        <button
                          className="btn btn-sm btn-primary"
                          onClick={handleSaveEdit}
                        >
                          Сохранить
                        </button>
                        <button
                          className="btn btn-sm btn-ghost"
                          onClick={handleCancelEdit}
                        >
                          Отмена
                        </button>
                      </div>
                    </div>
                  ) : (
                    <p className="text-base-content">{comment.text}</p>
                  )}

                  {/* Аттачменты комментария */}
                  <div className="mt-2">
                    <FilePreview
                      attachments={comment.attachments || []}
                      reportId={reportId}
                      bugId={bugId}
                      attachType={2}
                      commentId={comment.id}
                      onAttachmentUpload={(file: File) =>
                        addCommentAttachment({
                          reportId,
                          bugId,
                          commentId: comment.id,
                          file,
                        })
                      }
                      onAttachmentDelete={(attachmentId: number) =>
                        handleDeleteCommentAttachment(comment.id, attachmentId)
                      }
                    />
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Форма добавления комментария */}
      <div className="flex items-start gap-3">
        {/* Аватар */}
        <Avatar width={8} />

        <div className="flex-1">
          <div className="flex gap-2 items-center">
            <textarea
              value={newCommentText}
              onChange={(e) => setNewCommentText(e.target.value)}
              onKeyDown={handleKeyDownNewComment}
              placeholder="Оставьте сообщение..."
              className="textarea textarea-bordered resize-none min-h-auto flex-1"
              rows={1}
              disabled={isSubmitting}
            />

            {/* Кнопки действий */}
            <div className="flex flex-row gap-1 items-center">
              <div className="p-2">
                <AttachFileButton
                  onClick={() => fileInputRef.current?.click()}
                  disabled={isSubmitting}
                />
              </div>

              {/* Кнопка отправки */}
              <button
                className="btn btn-primary btn-sm p-2 btn-circle"
                onClick={handleSubmitComment}
                disabled={
                  isSubmitting ||
                  (!newCommentText.trim() && commentAttachments.length === 0)
                }
                title="Отправить"
              >
                <Send className="w-4 h-4" />
              </button>
            </div>
          </div>

          {/* Аттачменты к новому комментарию (ниже текстового поля и кнопок) */}
          {!!commentAttachments.length && (
            <div className="flex flex-wrap gap-2 mt-2">
              {commentAttachments.map((file, index) => (
                <div key={index} className="relative group">
                  <span className="text-xs bg-base-200 px-2 py-1 rounded">
                    {file.name}
                  </span>
                  <button
                    className="absolute -top-1 -right-1 bg-error text-error-content rounded-full p-0.5 opacity-0 group-hover:opacity-100 transition-opacity duration-200 hover:bg-error-focus"
                    onClick={() => handleRemoveAttachment(index)}
                    title="Убрать файл"
                  >
                    <Trash2 className="w-3 h-3" />
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Скрытый input для файлов */}
      <input
        ref={fileInputRef}
        type="file"
        multiple
        style={{ display: "none" }}
        onChange={handleFileChange}
      />
    </div>
  );
};

export default Comments;
