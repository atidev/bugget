import { useState } from "react";
import { MessageCircle } from "lucide-react";
import { useStoreMap, useUnit } from "effector-react";
import {
  $commentsByBugId,
  createCommentFx,
  deleteCommentEvent,
  updateCommentEvent,
  createCommentAttachmentFx,
  deleteCommentAttachmentFx,
} from "@/store/comments";
import Comment from "./components/Comment/Comment";
import NewCommentForm from "./components/NewCommentForm/NewCommentForm";

type Props = {
  reportId: number;
  bugId: number;
};

const Comments = ({ reportId, bugId }: Props) => {
  const [newCommentText, setNewCommentText] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [commentAttachments, setCommentAttachments] = useState<File[]>([]);

  const comments = useStoreMap({
    store: $commentsByBugId,
    keys: [bugId],
    fn: (state, [id]) => state[id] || [],
  });

  const addAttachment = useUnit(createCommentAttachmentFx);
  const removeAttachment = useUnit(deleteCommentAttachmentFx);

  const handleUpdate = (commentId: number, text: string) => {
    updateCommentEvent({
      reportId,
      bugId,
      commentId,
      text,
    });
  };

  const handleSubmit = async () => {
    if (!newCommentText.trim() && commentAttachments.length === 0) return;

    setIsSubmitting(true);
    try {
      const created = await createCommentFx({
        reportId,
        bugId,
        text: newCommentText.trim() || "Файл прикреплен",
      });

      if (created?.id && commentAttachments.length > 0) {
        await Promise.all(
          commentAttachments.map((file) =>
            addAttachment({
              reportId,
              bugId,
              commentId: created.id,
              file,
            })
          )
        );
      }

      setNewCommentText("");
      setCommentAttachments([]);
    } catch (error) {
      console.error("Ошибка при создании комментария:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (commentId: number) => {
    try {
      await deleteCommentEvent({ reportId, bugId, commentId });
    } catch (error) {
      console.error("Ошибка при удалении комментария:", error);
    }
  };

  const handleFileChange = (files: FileList | null) => {
    if (!files) return;
    setCommentAttachments((prev) => [...prev, ...Array.from(files)]);
  };

  const handleRemoveAttachment = (index: number) => {
    setCommentAttachments((prev) => prev.filter((_, i) => i !== index));
  };

  const handleUploadAttachment = (commentId: number, file: File) => {
    addAttachment({
      reportId,
      bugId,
      commentId,
      file,
    });
  };

  const handleDeleteAttachment = (commentId: number, attachmentId: number) => {
    removeAttachment({ reportId, bugId, commentId, attachmentId });
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
            <Comment
              key={comment.id}
              reportId={reportId}
              bugId={bugId}
              id={comment.id}
              text={comment.text}
              creatorUserId={comment.creatorUserId}
              createdAt={comment.createdAt}
              attachments={comment.attachments}
              onDeleteComment={handleDelete}
              onUpdateComment={handleUpdate}
              onUploadAttachment={handleUploadAttachment}
              onDeleteAttachment={handleDeleteAttachment}
            />
          ))}
        </div>
      )}

      <NewCommentForm
        value={newCommentText}
        attachments={commentAttachments}
        isSubmitting={isSubmitting}
        onChange={setNewCommentText}
        onSubmit={handleSubmit}
        onPickFiles={handleFileChange}
        onRemovePicked={handleRemoveAttachment}
      />
    </div>
  );
};

export default Comments;
