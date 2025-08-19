import { memo, useRef, useState, useCallback } from "react";
import { Send, Trash2 } from "lucide-react";
import { useUnit } from "effector-react";
import { createCommentFx, createCommentAttachmentFx } from "@/store/comments";
import AttachFileButton from "@/components/AttachFileButton/AttachFileButton";
import Avatar from "@/components/Avatar/Avatar";

type Props = {
  reportId: number;
  bugId: number;
};

const NewCommentForm = memo((props: Props) => {
  const { reportId, bugId } = props;

  const [text, setText] = useState("");
  const [attachments, setAttachments] = useState<File[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const createComment = useUnit(createCommentFx);
  const addAttachment = useUnit(createCommentAttachmentFx);

  const handleSubmit = useCallback(async () => {
    if (!text.trim() && attachments.length === 0) return;

    setIsSubmitting(true);
    try {
      const created = await createComment({
        reportId,
        bugId,
        text: text.trim() || "Файл прикреплен",
      });

      if (created?.id && attachments.length > 0) {
        await Promise.all(
          attachments.map((file) =>
            addAttachment({
              reportId,
              bugId,
              commentId: created.id,
              file,
            })
          )
        );
      }

      setText("");
      setAttachments([]);
    } catch (error) {
      console.error("Ошибка при создании комментария:", error);
    } finally {
      setIsSubmitting(false);
    }
  }, [reportId, bugId, text, attachments, createComment, addAttachment]);

  const handleFileChange = useCallback((files: FileList | null) => {
    if (!files) return;
    setAttachments((prev) => [...prev, ...Array.from(files)]);
  }, []);

  const handleRemoveAttachment = useCallback((index: number) => {
    setAttachments((prev) => prev.filter((_, i) => i !== index));
  }, []);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      if (text.trim() || attachments.length > 0) handleSubmit();
    }
  };

  return (
    <div className="flex items-start gap-3">
      <Avatar width={8} />
      <div className="flex-1">
        <div className="flex gap-2 items-center">
          <textarea
            value={text}
            onChange={(e) => setText(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Оставьте сообщение..."
            className="textarea textarea-bordered resize-none min-h-auto flex-1"
            rows={1}
            disabled={isSubmitting}
          />
          <div className="flex flex-row gap-1 items-center">
            <div className="p-2">
              <AttachFileButton
                onClick={() => fileInputRef.current?.click()}
                disabled={isSubmitting}
              />
            </div>
            <button
              className="btn btn-primary btn-sm p-2 btn-circle"
              onClick={handleSubmit}
              disabled={
                isSubmitting || (!text.trim() && attachments.length === 0)
              }
              title="Отправить"
            >
              <Send className="w-4 h-4" />
            </button>
          </div>
        </div>
        {!!attachments.length && (
          <div className="flex flex-wrap gap-2 mt-2">
            {attachments.map((file, index) => (
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
        <input
          ref={fileInputRef}
          type="file"
          multiple
          style={{ display: "none" }}
          onChange={(e) => handleFileChange(e.target.files)}
        />
      </div>
    </div>
  );
});

export default NewCommentForm;
