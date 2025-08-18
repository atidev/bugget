import { memo, useState } from "react";
import { MoreVertical } from "lucide-react";
import Avatar from "@/components/Avatar/Avatar";
import { Attachment } from "@/types/attachment";
import getCommentTimeDisplay from "@/utils/dates/getCommentTimeDisplay";
import { useUserDisplayName } from "@/hooks/useUserDisplayName";
import FilePreview from "../../../FilePreview/FilePreview";

type CommentItemProps = {
  reportId: number;
  bugId: number;
  id: number;
  text: string;
  creatorUserId: string;
  createdAt: string;
  attachments?: Attachment[] | null;
  onDeleteComment: (commentId: number) => void;
  onUpdateComment: (commentId: number, text: string) => void;
  onUploadAttachment: (commentId: number, file: File) => void;
  onDeleteAttachment: (commentId: number, attachmentId: number) => void;
};

const Comment = memo((props: CommentItemProps) => {
  const {
    reportId,
    bugId,
    id,
    text,
    creatorUserId,
    createdAt,
    attachments,
    onDeleteComment,
    onUpdateComment,
    onUploadAttachment,
    onDeleteAttachment,
  } = props;

  const userDisplayName = useUserDisplayName(creatorUserId);

  const [isEditing, setIsEditing] = useState(false);
  const [draft, setDraft] = useState(text);

  const saveEdit = () => {
    if (!draft.trim()) return;
    onUpdateComment(id, draft.trim());
    setIsEditing(false);
  };

  return (
    <div className="border-b border-base-300">
      <div className="flex items-start gap-3">
        <Avatar width={8} />
        <div className="flex-1 min-w-0 mb-2">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <span className="text-xs font-medium text-base-content">
                {userDisplayName}
              </span>
              <span className="text-xs text-base-content/60">
                {getCommentTimeDisplay(createdAt)}
              </span>
            </div>
            <div className="dropdown dropdown-end">
              <button className="btn btn-ghost btn-xs p-1">
                <MoreVertical className="w-4 h-4" />
              </button>
              <ul className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-32">
                <li>
                  <button
                    className="text-info"
                    onClick={() => setIsEditing(true)}
                  >
                    Редактировать
                  </button>
                </li>
                <li>
                  <button
                    className="text-error"
                    onClick={() => onDeleteComment(id)}
                  >
                    Удалить
                  </button>
                </li>
              </ul>
            </div>
          </div>

          {isEditing ? (
            <div className="space-y-2">
              <textarea
                value={draft}
                onChange={(e) => setDraft(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter" && !e.shiftKey) {
                    e.preventDefault();
                    saveEdit();
                  }
                }}
                className="textarea textarea-bordered w-full resize-none min-h-auto"
                placeholder="Введите текст комментария... (Enter для сохранения)"
                rows={1}
              />
              <div className="flex gap-2">
                <button className="btn btn-sm btn-primary" onClick={saveEdit}>
                  Сохранить
                </button>
                <button
                  className="btn btn-sm btn-ghost"
                  onClick={() => {
                    setIsEditing(false);
                    setDraft(text);
                  }}
                >
                  Отмена
                </button>
              </div>
            </div>
          ) : (
            <p className="text-base-content">{text}</p>
          )}

          <div className="mt-2">
            <FilePreview
              attachments={attachments || []}
              reportId={reportId}
              bugId={bugId}
              attachType={2}
              onAttachmentUpload={(file: File) => onUploadAttachment(id, file)}
              onAttachmentDelete={(attachmentId: number) =>
                onDeleteAttachment(id, attachmentId)
              }
              commentId={id}
            />
          </div>
        </div>
      </div>
    </div>
  );
});

export default Comment;
